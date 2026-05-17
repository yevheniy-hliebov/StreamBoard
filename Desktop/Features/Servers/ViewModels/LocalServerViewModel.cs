using StreamTabula.Core;
using StreamTabula.Core.Models;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Servers.ViewModels
{
    public class LocalServerViewModel : ObservableObject
    {
        private readonly ServerConfigsStorage _storage;
        private readonly LocalServer _server;

        public ObservableCollection<HttpRequestLog> HttpRequestLogs { get; } = new();

        public LocalServerViewModel(ServerConfigsStorage storage, LocalServer server)
        {
            _storage = storage;
            _server = server;

            _server.StatusChanged += OnServerStatusChanged;
            _server.RequestProcessed += OnRequestProcessed;
        }

        private void OnServerStatusChanged(ServerStatus status)
        {
            OnPropertyChanged(nameof(ServerStatusString));
            OnPropertyChanged(nameof(ActionButtonText));
            OnPropertyChanged(nameof(ActionButtonIcon));
            OnPropertyChanged(nameof(IsProcessing));
            OnPropertyChanged(nameof(CanEditSettings));
        }

        private void OnRequestProcessed(HttpRequestLog log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                HttpRequestLogs.Insert(0, log);

                if (HttpRequestLogs.Count > 50) HttpRequestLogs.RemoveAt(50);
            });
        }

        // Start / Stop Server

        public string ServerStatusString
        {
            get
            {
                if (_server.Status == ServerStatus.Starting)
                {
                    return "Starting...";
                }
                if (_server.Status == ServerStatus.Stopping)
                {
                    return "Stopping...";
                }
                return _server.Status.ToString();
            }
        }

        public string ActionButtonText => _server.Status switch
        {
            ServerStatus.Running => "Stop Server",
            ServerStatus.Stopped => "Start Server",
            ServerStatus.Starting => "Starting...",
            ServerStatus.Stopping => "Stopping...",
            _ => "Status Unknown"
        };

        public FluentIconType ActionButtonIcon => _server.Status switch
        {
            ServerStatus.Running => FluentIconType.Stop,
            ServerStatus.Stopped => FluentIconType.PowerButton,
            ServerStatus.Starting => FluentIconType.Sync,
            ServerStatus.Stopping => FluentIconType.Sync,
            _ => FluentIconType.Help
        };

        public bool IsProcessing => _server.Status == ServerStatus.Starting || _server.Status == ServerStatus.Stopping;

        public ICommand ToggleServerCommand => new RelayCommand<object>(async _ =>
        {
            if (IsProcessing) return;

            try
            {
                if (_server.Status == ServerStatus.Running)
                    await _server.Stop();
                else
                    await _server.Start();
            }
            catch (Exception ex)
            {
                string message = ex switch
                {
                    UnauthorizedAccessException => "Access denied. Run the application as Administrator to use this IP address.",
                    InvalidOperationException => ex.Message,
                    _ => $"Unexpected error: {ex.Message}"
                };

                var uiMessageBox = new MessageBox
                {
                    Title = "Server Error",
                    Content = message,
                    CloseButtonText = "Close",
                    MaxWidth = 400
                };

                uiMessageBox.Owner = System.Windows.Application.Current.MainWindow;

                await uiMessageBox.ShowDialogAsync();
            }
        });

        // Auto Start Toggle
        public bool AutoStart
        {
            get => _storage.Current.Local.AutoStart;
            set
            {
                if (_storage.Current.Local.AutoStart == value) return;
                _storage.Current.Local.AutoStart = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        // NetworkIpAddress
        public string NetworkIpAddress
        {
            get
            {
                return NetworkHelper.GetLocalIpAddress();
            }
        }

        // Port
        public int Port
        {
            get => _storage.Current.Local.Port;
            set
            {
                if (_storage.Current.Local.Port != value)
                {
                    _storage.Current.Local.Port = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool CanEditSettings => _server.Status == ServerStatus.Stopped;
    }
}
