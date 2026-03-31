using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using StreamBoard.Core;
using StreamBoard.Features.Integrations.Common.Models;
using StreamBoard.Features.Integrations.Obs.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StreamBoard.Features.Integrations.Obs.Services
{
    public class ObsService : ObservableObject
    {
        public readonly OBSWebsocket Obs;
        private readonly ObsConnectionSettings _settings;
        private CancellationTokenSource? _reconnectCts;

        private ConnectionState _connectionState = ConnectionState.NotConnected;
        public ConnectionState ConnectionState
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

        public ObsService(ObsConnectionSettings settings)
        {
            Obs = new OBSWebsocket();
            _settings = settings;

            Obs.Connected += OnConnected;
            Obs.Disconnected += OnDisconnected;
        }

        public void Connect()
        {
            if (Obs.IsConnected || ConnectionState == ConnectionState.Connecting)
                return;

            StopReconnect();
            ConnectionState = ConnectionState.Connecting;

            try
            {
                var url = $"ws://{_settings.Address}:{_settings.Port}";
                Obs.ConnectAsync(url, _settings.Password);
            }
            catch
            {
                ConnectionState = ConnectionState.Failed;
                if (_settings.AutoReconnect) TryReconnect();
            }
        }

        public void Disconnect()
        {
            StopReconnect();

            if (Obs.IsConnected)
            {
                ConnectionState = ConnectionState.Disconnecting;
                Obs.Disconnect();
            }
            else
            {
                ConnectionState = ConnectionState.NotConnected;
            }
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            StopReconnect();
            ConnectionState = ConnectionState.Connected;
        }

        private void OnDisconnected(object? sender, ObsDisconnectionInfo e)
        {
            // Якщо ми ініціювали відключення самі
            if (ConnectionState == ConnectionState.Disconnecting)
            {
                ConnectionState = ConnectionState.NotConnected;
                return;
            }

            ConnectionState = ConnectionState.Failed;

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

                        ConnectionState = ConnectionState.Connecting;
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
}