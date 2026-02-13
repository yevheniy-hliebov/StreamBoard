using StreamBoard.Features.Servers.Models;
using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Services
{
    public enum ServerStatus
    {
        Stopped, Starting, Running, Stopping
    }
    public class HttpServer
    {
        private readonly HttpServerConfig _config;
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

        public HttpServer(HttpServerConfig config)
        {
            _config = config;
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
                catch (Exception ex)
                {
                    // Тут можна додати логування помилок циклу
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
                string text = $"Running on {_config.Address}:{_config.Port}";
                byte[] data = Encoding.UTF8.GetBytes(text);
                ctx.Response.ContentType = "text/plain";
                ctx.Response.ContentLength64 = data.Length;
                await ctx.Response.OutputStream.WriteAsync(data);
            }
            finally
            {
                ctx.Response.OutputStream.Close();
            }
        }
    }
}
