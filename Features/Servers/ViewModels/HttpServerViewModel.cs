using StreamBoard.Core;
using StreamBoard.Features.Servers.Services;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Servers.ViewModels
{
    public class HttpServerViewModel : ObservableObject
    {
        private readonly ServerConfigsStorage _storage;
        private readonly HttpServer _server;

        public HttpServerViewModel(ServerConfigsStorage storage, HttpServer server)
        {
            _storage = storage;
            _server = server;

            _server.StatusChanged += OnServerStatusChanged;
        }

        private void OnServerStatusChanged(ServerStatus status)
        {
            OnPropertyChanged(nameof(ServerStatusString));
            OnPropertyChanged(nameof(ActionButtonText));
            OnPropertyChanged(nameof(ActionButtonIcon));
            OnPropertyChanged(nameof(IsProcessing));
            OnPropertyChanged(nameof(CanEditSettings));
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

        public SymbolRegular ActionButtonIcon => _server.Status switch
        {
            ServerStatus.Running => SymbolRegular.RecordStop24,
            ServerStatus.Stopped => SymbolRegular.Power24,
            ServerStatus.Starting => SymbolRegular.ArrowSync24,
            ServerStatus.Stopping => SymbolRegular.ArrowSync24,
            _ => SymbolRegular.Question24
        };

        public bool IsProcessing => _server.Status == ServerStatus.Starting || _server.Status == ServerStatus.Stopping;

        public ICommand ToggleServerCommand => new RelayCommand<object>(async _ =>
        {
            if (IsProcessing) return;

            if (_server.Status == ServerStatus.Running)
            {
                await _server.StopAsync();
            }
            else if (_server.Status == ServerStatus.Stopped)
            {
                await _server.StartAsync();
            }
        });

        // Auto Start Toggle
        public bool AutoStart
        {
            get => _storage.Current.Http.AutoStart;
            set
            {
                if (_storage.Current.Http.AutoStart == value) return;
                _storage.Current.Http.AutoStart = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        // Address
        public string Address
        {
            get => _storage.Current.Http.Address;
            set
            {
                if (_storage.Current.Http.Address != value)
                {
                    _storage.Current.Http.Address = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        // Port
        public int Port
        {
            get => _storage.Current.Http.Port;
            set
            {
                if (_storage.Current.Http.Port != value)
                {
                    _storage.Current.Http.Port = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool CanEditSettings => _server.Status == ServerStatus.Stopped;
    }
}
