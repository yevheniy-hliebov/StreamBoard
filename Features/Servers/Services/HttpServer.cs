using StreamBoard.Features.Servers.Models;
using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Services
{
    public class HttpServer
    {
        private readonly HttpServerConfig _config;
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;

        public bool IsRunning => _listener?.IsListening == true;

        public HttpServer(HttpServerConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            if (IsRunning) return;

            _listener = new HttpListener();
            _listener.Prefixes.Add(_config.Prefix);

            _cts = new();

            _listener.Start();

            Task.Run(() => ListenLoop(_cts.Token));
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
                catch when (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;

            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
        }

        private async Task Handle(HttpListenerContext ctx)
        {
            string text = $"Running on {_config.Address}:{_config.Port}";
            byte[] data = Encoding.UTF8.GetBytes(text);

            ctx.Response.ContentType = "text/plain";
            ctx.Response.ContentLength64 = data.Length;

            await ctx.Response.OutputStream.WriteAsync(data);
            ctx.Response.OutputStream.Close();
        }
    }
}
