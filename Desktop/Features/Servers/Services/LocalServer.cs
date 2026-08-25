using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StreamTabula.Features.Servers.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace StreamTabula.Features.Servers.Services;

public class LocalServer
{
    private readonly LocalServerConfig _config;
    private readonly HttpRouter _router;
    private readonly IWebSocketBroadcaster? _wsBroadcaster;
    private readonly Lock _statusLock = new();

    private WebApplication? _app;

    public IPAddress LocalIPAddress { get; }

    public event Action<ServerStatus>? StatusChanged;
    public event Action<HttpRequestLog>? RequestProcessed;

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

    public LocalServer(ILocalIpAddressResolver ipResolver, LocalServerConfig config, HttpRouter router, IWebSocketBroadcaster? wsBroadcater = null)
    {
        LocalIPAddress = ipResolver.Get();

        _config = config;
        _router = router;
        _wsBroadcaster = wsBroadcater;
    }

    public async Task Start()
    {
        lock (_statusLock)
        {
            if (Status != ServerStatus.Stopped) return;
            Status = ServerStatus.Starting;
        }

        try
        {
            if (IsPortInUse(_config.Port))
            {
                throw new InvalidOperationException($"Port {_config.Port} is already in use by another application.");
            }

            var builder = WebApplication.CreateBuilder();

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(_config.Port);
            });

            _app = builder.Build();

            _app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Accept, Host");

                if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }

                await next();
            });

            _app.UseWebSockets();

            _app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    _wsBroadcaster?.AddClient(webSocket);
                    try
                    {
                        await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
                    }
                    catch (OperationCanceledException) { }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });

            _app.MapFallback(ProcessHttpRequest);

            await _app.StartAsync();

            Status = ServerStatus.Running;
        }
        catch (System.IO.IOException ex)
        {
            Status = ServerStatus.Stopped;

            if (_app != null)
            {
                await _app.DisposeAsync();
                _app = null;
            }

            if (ex.InnerException is System.Net.Sockets.SocketException socketEx &&
                socketEx.SocketErrorCode == System.Net.Sockets.SocketError.AddressAlreadyInUse)
            {
                throw new InvalidOperationException($"Port {_config.Port} is already in use by another application.", ex);
            }

            throw new InvalidOperationException($"Failed to bind to port {_config.Port}.", ex);
        }
        catch (Exception)
        {
            Status = ServerStatus.Stopped;

            if (_app != null)
            {
                await _app.DisposeAsync();
                _app = null;
            }

            throw;
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
            if (_app != null)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
                await _app.StopAsync(cts.Token);

                await _app.DisposeAsync();
                _app = null;
            }
        }
        finally
        {
            Status = ServerStatus.Stopped;
        }
    }

    private async Task ProcessHttpRequest(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";
        var controller = _router.Resolve(path);

        if (controller == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("404 - Not Found");
            return;
        }

        try
        {
            await controller.HandleAsync(context);
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("500 - Internal Server Error");
        }
        finally
        {
            RequestProcessed?.Invoke(new HttpRequestLog(context));
        }
    }

    private static bool IsPortInUse(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();

        foreach (var endpoint in tcpListeners)
        {
            if (endpoint.Port == port)
            {
                return true;
            }
        }

        return false;
    }
}