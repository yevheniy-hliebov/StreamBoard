using StreamBoard.Features.Servers.Models;
using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Services
{
    public enum ServerStatus
    {
        Stopped, Starting, Running, Stopping
    }

    public record HttpRequestLog(string Method, string Endpoint, string IpAddress, int StatusCode, DateTime Timestamp);

    public class HttpServer
    {
        private readonly HttpServerConfig _config;

        private readonly IEnumerable<IHttpController> _controllers;
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;

        private readonly object _statusLock = new();

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

        public HttpServer(HttpServerConfig config, IEnumerable<IHttpController> controllers)
        {
            _config = config;
            _controllers = controllers;
        }

        public async Task StartAsync()
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

                _ = Task.Run(() => ListenLoop(_cts.Token));

                Status = ServerStatus.Running;
            }
            catch (HttpListenerException ex)
            {
                Status = ServerStatus.Stopped;
                if (ex.ErrorCode == 5)
                    throw new UnauthorizedAccessException("Access denied. Run as admin to use this IP/Port.", ex);
                if (ex.ErrorCode == 183)
                    throw new InvalidOperationException("Port is already in use by another application.", ex);

                throw;
            }
            catch (Exception)
            {
                Status = ServerStatus.Stopped;
                throw;
            }
        }

        public async Task StopAsync()
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
                    var ctx = await _listener!.GetContextAsync();
                    _ = Task.Run(() => Handle(ctx));
                }
                catch when (token.IsCancellationRequested) { break; }
                catch
                {
                    if (_listener?.IsListening != true) break;
                }
            }
        }

        public void Start() => StartAsync().GetAwaiter().GetResult();
        public void Stop() => StopAsync().GetAwaiter().GetResult();

        private async Task Handle(HttpListenerContext ctx)
        {
            try
            {
                var path = ctx.Request.Url?.AbsolutePath ?? "/";

                var controller = _controllers.FirstOrDefault(c => c.CanHandle(path));

                if (controller != null)
                {
                    await controller.HandleAsync(ctx);
                }
                else
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    byte[] data = Encoding.UTF8.GetBytes("404 - Not Found");
                    await ctx.Response.OutputStream.WriteAsync(data);
                }
            }
            catch
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                string ip = ctx.Request.RemoteEndPoint?.Address.ToString() ?? "Unknown";

                if (ip == "::1") ip = "127.0.0.1";

                var log = new HttpRequestLog(
                    ctx.Request.HttpMethod, 
                    ctx.Request.Url?.AbsolutePath ?? "/",
                    ip, 
                    ctx.Response.StatusCode, 
                    DateTime.Now
                );

                RequestProcessed?.Invoke(log);

                ctx.Response.OutputStream.Close();
            }
        }
    }
}
