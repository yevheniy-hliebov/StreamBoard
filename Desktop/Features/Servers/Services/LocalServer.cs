using StreamTabula.Core.Models;
using StreamTabula.Features.Servers.Models;
using System.Net;
using System.Text;

namespace StreamTabula.Features.Servers.Services
{
    public class LocalServer(LocalServerConfig config, HttpRouter router, WebsocketManager? wsManager = null)
    {
        private readonly LocalServerConfig _config = config;
        private readonly HttpRouter _router = router;
        private readonly WebsocketManager? _wsManager = wsManager;
        private readonly Lock _statusLock = new();

        private HttpListener? _listener;
        private CancellationTokenSource? _cts;
        private Task? _listeningTask;

        public event Action<ServerStatus>? StatusChanged;

        private ServerStatus _status = ServerStatus.Stopped;
        public ServerStatus Status
        {
            get => _status;
            private set
            {
                if (_status == value) return;
                _status = value;
                StatusChanged?.Invoke(_status);
            }
        }

        public bool IsRunning => Status == ServerStatus.Running;
        public bool ShouldAutoStart => _config.AutoStart;

        public event Action<HttpRequestLog>? RequestProcessed;

        public async Task Start()
        {
            lock (_statusLock)
            {
                if (Status != ServerStatus.Stopped) return;
                Status = ServerStatus.Starting;
            }

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(_config.HttpPrefix);
                _cts = new CancellationTokenSource();

                _listener.Start();

                _listeningTask = Task.Run(() => ListenLoop(_cts.Token));

                Status = ServerStatus.Running;
            }
            catch (Exception ex)
            {
                Status = ServerStatus.Stopped;
                HandleStartupException(ex);
            }
        }

        public async Task Stop()
        {
            lock (_statusLock)
            {
                if (Status != ServerStatus.Running) return;
                Status = ServerStatus.Stopping;
            }

            try
            {
                _cts?.Cancel();
                _listener?.Stop();

                if (_listeningTask != null)
                    await Task.WhenAny(_listeningTask, Task.Delay(5000));

                _listener?.Close();
            }
            finally
            {
                Status = ServerStatus.Stopped;
            }
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var listener = _listener;
                    if (listener == null)
                        return;

                    var ctx = await listener.GetContextAsync();
                    _ = Task.Run(() => Handle(ctx), token);
                }
                catch when (token.IsCancellationRequested) { break; }
                catch
                {
                    if (_listener?.IsListening != true) break;
                }
            }
        }

        private async Task Handle(HttpListenerContext ctx)
        {
            try
            {
                if (_wsManager != null && ctx.Request.IsWebSocketRequest)
                {
                    if (ctx.Request.Url?.AbsolutePath.Equals("/ws", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var wsContext = await ctx.AcceptWebSocketAsync(subProtocol: null);
                        _wsManager?.AddClient(wsContext.WebSocket);
                        return;
                    }
                }

                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, Host");

                if (ctx.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }

                await ProcessRequest(ctx);
            }
            catch
            {
                if (!ctx.Request.IsWebSocketRequest)
                    await WriteTextResponse(ctx, HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            finally
            {
                if (!ctx.Request.IsWebSocketRequest)
                {
                    RequestProcessed?.Invoke(new HttpRequestLog(ctx));
                    ctx.Response.OutputStream.Close();
                }
            }
        }

        private async Task ProcessRequest(HttpListenerContext ctx)
        {
            var path = ctx.Request.Url?.AbsolutePath ?? "/";

            var controller = _router.Resolve(path);

            if (controller == null)
            {
                await WriteTextResponse(ctx, HttpStatusCode.NotFound, "Not Found");
                return;
            }

            await controller.HandleAsync(ctx);
        }

        private static async Task WriteTextResponse(
            HttpListenerContext ctx,
            HttpStatusCode status,
            string text
        )
        {
            int statusCode = (int)status;
            ctx.Response.StatusCode = statusCode;

            var data = Encoding.UTF8.GetBytes($"{statusCode} - {text}");
            await ctx.Response.OutputStream.WriteAsync(data);
        }

        private static void HandleStartupException(Exception ex)
        {
            if (ex is HttpListenerException httpEx)
            {
                switch (httpEx.ErrorCode)
                {
                    case (int)WinErrorCodes.AccessDenied:
                        throw new UnauthorizedAccessException("Access denied. Run as admin to use this IP/Port.", ex);
                    case (int)WinErrorCodes.AlreadyExists:
                        throw new InvalidOperationException("Port is already in use by another application.", ex);
                }
            }
            throw ex;
        }
    }
}
