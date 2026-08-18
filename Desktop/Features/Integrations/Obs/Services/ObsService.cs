using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Obs.Models;

namespace StreamTabula.Features.Integrations.Obs.Services;

public class ObsService : ObservableObject
{
    private readonly IOBSWebsocket _obs;
    private readonly ObsConnectionSettings _settings;
    private CancellationTokenSource? _reconnectCts;

    private ConnectionStatus _connectionState = ConnectionStatus.NotConnected;
    public ConnectionStatus ConnectionState
    {
        get => _connectionState;
        private set
        {
            if (SetProperty(ref _connectionState, value))
            {
                OnPropertyChanged(nameof(IsConnected));
            }
        }
    }

    public bool IsConnected => _obs.IsConnected;

    public ObsService(IOBSWebsocket obs,  ObsConnectionSettings settings)
    {
        _obs = obs;

        _settings = settings;

        _obs.Connected += OnConnected;
        _obs.Disconnected += OnDisconnected;
    }

    public void Connect()
    {
        if (_obs.IsConnected || ConnectionState == ConnectionStatus.Connecting)
            return;

        StopReconnect();
        ConnectionState = ConnectionStatus.Connecting;

        try
        {
            var url = $"ws://{_settings.Address}:{_settings.Port}";
            _obs.ConnectAsync(url, _settings.Password);
        }
        catch
        {
            ConnectionState = ConnectionStatus.Failed;
            if (_settings.AutoReconnect) TryReconnect();
        }
    }

    public void Disconnect()
    {
        StopReconnect();

        if (_obs.IsConnected)
        {
            ConnectionState = ConnectionStatus.Disconnecting;
            _obs.Disconnect();
        }
        else
        {
            ConnectionState = ConnectionStatus.NotConnected;
        }
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        StopReconnect();
        ConnectionState = ConnectionStatus.Connected;
    }

    private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
    {
        if (ConnectionState == ConnectionStatus.Disconnecting)
        {
            ConnectionState = ConnectionStatus.NotConnected;
            return;
        }

        ConnectionState = ConnectionStatus.Failed;

        if (_settings.AutoReconnect)
            TryReconnect();
    }

    private void TryReconnect()
    {
        if (_reconnectCts != null && !_reconnectCts.IsCancellationRequested) return;

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

                    ConnectionState = ConnectionStatus.Connecting;
                    var url = $"ws://{_settings.Address}:{_settings.Port}";
                    _obs.ConnectAsync(url, _settings.Password);

                    await Task.Delay(5000, token);
                }
                catch (TaskCanceledException) { break; }
                catch { }
            }
        }, token);
    }

    private void StopReconnect()
    {
        _reconnectCts?.Cancel();
        _reconnectCts = null;
    }
}