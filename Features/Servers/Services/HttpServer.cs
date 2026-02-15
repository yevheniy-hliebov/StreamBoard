using StreamBoard.Core.Models;
using StreamBoard.Features.Servers.Models;
using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Services
{
    public class HttpServer(HttpServerConfig config, HttpRouter router)
    {
        private readonly HttpServerConfig _config = config;
        private readonly HttpRouter _router = router;
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
                _listener.Prefixes.Add(_config.Prefix);
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
                await ProcessRequest(ctx);
            }
            catch
            {
                await WriteTextResponse(ctx, HttpStatusCode.InternalServerError, "Internal Server Error");
            }
            finally
            {
                RequestProcessed?.Invoke(new HttpRequestLog(ctx));
                ctx.Response.OutputStream.Close();
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
            ctx.Response.StatusCode = (int)status;

            var data = Encoding.UTF8.GetBytes($"{status} - {text}");
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
