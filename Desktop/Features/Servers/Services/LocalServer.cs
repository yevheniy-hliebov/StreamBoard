using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Servers.Models;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StreamTabula.Features.Servers.Services;

public interface ISystemServer : ILocalServer { }
public interface IMobileServer : ILocalServer { }

public interface ILocalServer
{
    IPAddress Address { get; }
    ServerStatus Status { get; }
    bool IsRunning { get; }
    bool ShouldAutoStart { get; }

    event Action<ServerStatus>? StatusChanged;
    event Action<HttpRequestLog>? RequestProcessed;

    Task StartAsync();
    Task StopAsync();
}

public class SystemServer(IPAddress address, LocalServerConfig config, Type[] controllers, IServiceProvider parentServices)
        : LocalServer(address, config, controllers, parentServices, null), ISystemServer;

public class MobileServer(IPAddress address, LocalServerConfig config, Type[] controllers, IServiceProvider parentServices, IWebSocketBroadcaster wsBroadcaster)
    : LocalServer(address, config, controllers, parentServices, wsBroadcaster), IMobileServer;

public class LocalServer(
    IPAddress address,
    LocalServerConfig config,
    Type[] controllers,
    IServiceProvider parentServices,
    IWebSocketBroadcaster? wsBroadcaster = null) : ILocalServer
{
    private readonly Lock _statusLock = new();

    private WebApplication? _app;

    public IPAddress Address { get; } = address;

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
    public bool ShouldAutoStart => config.AutoStart;

    public event Action<ServerStatus>? StatusChanged;
    public event Action<HttpRequestLog>? RequestProcessed;

    public async Task StartAsync()
    {
        lock (_statusLock)
        {
            if (Status != ServerStatus.Stopped) return;
            Status = ServerStatus.Starting;
        }

        try
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(config.Port);
            });

            builder.Services.AddControllers().ConfigureApplicationPartManager(manager =>
                    {
                        var defaultProvider = manager.FeatureProviders.OfType<ControllerFeatureProvider>().FirstOrDefault();
                        if (defaultProvider != null)
                            manager.FeatureProviders.Remove(defaultProvider);

                        manager.FeatureProviders.Add(new AllowedControllersFeatureProvider(controllers));
                    })
                    .AddControllersAsServices();

            foreach (var type in controllers)
            {
                builder.Services.AddTransient(type, sp => parentServices.GetRequiredService(type));
            }

            _app = builder.Build();

            _app.UseCors();

            _app.Use(async (context, next) =>
            {
                try
                {
                    await next(context);
                }
                finally
                {
                    RequestProcessed?.Invoke(new HttpRequestLog(context));
                }
            });

            _app.UseWebSockets();

            if (wsBroadcaster != null)
            {
                _app.Map("/ws", HandleWebSocketRequest);
            }

            _app.MapControllers();

            await _app.StartAsync();
            Status = ServerStatus.Running;
        }
        catch (IOException ex) when (IsAddressInUseException(ex))
        {
            await CleanupAppAsync();
            throw new InvalidOperationException($"Port {config.Port} is already in use by another application.", ex);
        }
        catch (Exception)
        {
            await CleanupAppAsync();
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
            if (_app != null)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
                await _app.StopAsync(cts.Token);
                await CleanupAppAsync();
            }
        }
        finally
        {
            Status = ServerStatus.Stopped;
        }
    }

    private async Task CleanupAppAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
            _app = null;
        }
        Status = ServerStatus.Stopped;
    }

    private static bool IsAddressInUseException(IOException ex)
    {
        return ex.InnerException is SocketException socketEx &&
               socketEx.SocketErrorCode == SocketError.AddressAlreadyInUse;
    }

    private async Task HandleWebSocketRequest(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        wsBroadcaster?.AddClient(webSocket);
        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, context.RequestAborted);
        }
        catch (OperationCanceledException) { }
    }
}
