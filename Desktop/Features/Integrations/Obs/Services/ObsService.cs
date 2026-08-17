using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Obs.Models;

namespace StreamTabula.Features.Integrations.Obs.Services;

public class ObsService : ObservableObject
{
    public readonly IOBSWebsocket Obs;
    public readonly IOBSSceneService SceneService;
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

    public bool IsConnected => Obs.IsConnected;

    public ObsService(IOBSWebsocket obs, IOBSSceneService sceneService,  ObsConnectionSettings settings)
    {
        Obs = obs;
        SceneService = sceneService;

        _settings = settings;

        Obs.Connected += OnConnected;
        Obs.Disconnected += OnDisconnected;
    }

    public void Connect()
    {
        if (Obs.IsConnected || ConnectionState == ConnectionStatus.Connecting)
            return;

        StopReconnect();
        ConnectionState = ConnectionStatus.Connecting;

        try
        {
            var url = $"ws://{_settings.Address}:{_settings.Port}";
            Obs.ConnectAsync(url, _settings.Password);
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

        if (Obs.IsConnected)
        {
            ConnectionState = ConnectionStatus.Disconnecting;
            Obs.Disconnect();
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
        // Якщо ми ініціювали відключення самі
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

                    if (Obs.IsConnected) break;

                    ConnectionState = ConnectionStatus.Connecting;
                    var url = $"ws://{_settings.Address}:{_settings.Port}";
                    Obs.ConnectAsync(url, _settings.Password);

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