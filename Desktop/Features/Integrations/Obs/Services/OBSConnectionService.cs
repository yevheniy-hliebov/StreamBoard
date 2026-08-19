using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.OBS.Models;

namespace StreamTabula.Features.Integrations.OBS.Services;

public interface IOBSConnectionService : IDisposable
{
    ConnectionStatus Status { get; }
    bool IsConnected { get; }

    event Action<ConnectionStatus>? StatusChanged;

    Task ConnectAsync();
    void Disconnect();
}

public class OBSConnectionService : IOBSConnectionService
{
    private readonly IOBSWebsocket _obs;
    private readonly OBSConnectionSettings _settings;
    private CancellationTokenSource? _reconnectCts;

    public event Action<ConnectionStatus>? StatusChanged;

    private ConnectionStatus _status = ConnectionStatus.NotConnected;
    public ConnectionStatus Status
    {
        get => _status;
        private set
        {
            if (_status != value)
            {
                _status = value;
                StatusChanged?.Invoke(_status);
            }
        }
    }

    public bool IsConnected => _obs.IsConnected;

    private string ConnectionUrl => $"ws://{_settings.Address}:{_settings.Port}";

    public OBSConnectionService(IOBSWebsocket obs, OBSConnectionSettings settings)
    {
        _obs = obs;

        _settings = settings;

        _obs.Connected += OnConnected;
        _obs.Disconnected += OnDisconnected;
    }

    public async Task ConnectAsync()
    {
        if (_obs.IsConnected || Status == ConnectionStatus.Connecting)
            return;

        StopReconnect();
        Status = ConnectionStatus.Connecting;

        try
        {
            await Task.Run(() => _obs.ConnectAsync(ConnectionUrl, _settings.Password));
        }
        catch (Exception)
        {
            Status = ConnectionStatus.Failed;

            if (_settings.AutoReconnect)
            {
                TryReconnect();
            }
        }
    }

    public void Disconnect()
    {
        StopReconnect();

        if (_obs.IsConnected)
        {
            Status = ConnectionStatus.Disconnecting;
            _obs.Disconnect();
        }
        else
        {
            Status = ConnectionStatus.NotConnected;
        }
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        StopReconnect();
        Status = ConnectionStatus.Connected;
    }

    private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
    {
        if (Status == ConnectionStatus.Disconnecting)
        {
            Status = ConnectionStatus.NotConnected;
            return;
        }

        Status = ConnectionStatus.Failed;

        if (_settings.AutoReconnect)
        {
            TryReconnect();
        }
    }

    private void TryReconnect()
    {
        if (_reconnectCts != null && !_reconnectCts.IsCancellationRequested)
            return;

        _reconnectCts = new CancellationTokenSource();
        var token = _reconnectCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.ReconnectDelay), token);

                    if (_obs.IsConnected) break;

                    Status = ConnectionStatus.Connecting;
                    _obs.ConnectAsync(ConnectionUrl, _settings.Password);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    Status = ConnectionStatus.Failed;
                }
            }
        }, token);
    }

    private void StopReconnect()
    {
        if (_reconnectCts != null)
        {
            _reconnectCts.Cancel();
            _reconnectCts.Dispose();
            _reconnectCts = null;
        }
    }

    public void Dispose()
    {
        _obs.Connected -= OnConnected;
        _obs.Disconnected -= OnDisconnected;
        StopReconnect();
    }
}