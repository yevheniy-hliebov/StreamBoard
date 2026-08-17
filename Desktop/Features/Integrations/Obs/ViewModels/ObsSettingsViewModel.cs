using System.Windows.Input;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Common.Services;
using StreamTabula.Features.Integrations.Obs.Services;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Integrations.Obs.ViewModels
{
    public class ObsSettingsViewModel : ObservableObject
    {
        private IntegrationsStorage _storage;
        private ObsService _obsService;

        public ObsSettingsViewModel(IntegrationsStorage storage, ObsService obsService)
        {
            _storage = storage;
            _obsService = obsService;

            _obsService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ObsService.ConnectionState))
                {
                    OnPropertyChanged(nameof(ConnectionStateString));
                    OnPropertyChanged(nameof(ActionButtonText));
                    OnPropertyChanged(nameof(IsProcessing));
                    OnPropertyChanged(nameof(CanEditSettings));
                }
            };
        }

        public string ConnectionStateString
        {
            get
            {
                if (_obsService.ConnectionState == ConnectionStatus.NotConnected)
                {
                    return "Not connected";
                }
                if (_obsService.ConnectionState == ConnectionStatus.Connecting)
                {
                    return "Connecting...";
                }
                if (_obsService.ConnectionState == ConnectionStatus.Disconnecting)
                {
                    return "Disconnecting...";
                }
                return _obsService.ConnectionState.ToString();
            }
        }

        public string ActionButtonText => _obsService.ConnectionState switch
        {
            ConnectionStatus.NotConnected => "Connect",
            ConnectionStatus.Failed => "Connect",
            ConnectionStatus.Connecting => "Connecting...",
            ConnectionStatus.Connected => "Disconnect",
            ConnectionStatus.Disconnecting => "Disconnecting...",
            _ => "Status Unknown"
        };

        public bool IsProcessing =>
            _obsService.ConnectionState == ConnectionStatus.Connecting || _obsService.ConnectionState == ConnectionStatus.Disconnecting;

        public ICommand ToggleConnectionCommand => new RelayCommand<object>(async _ =>
        {
            if (IsProcessing) return;

            try
            {
                if (_obsService.IsConnected == true)
                    _obsService.Disconnect();
                else
                    _obsService.Connect();
            }
            catch (Exception ex)
            {
                string message = ex switch
                {
                    _ => $"Unexpected error: {ex.Message}"
                };

                var uiMessageBox = new MessageBox
                {
                    Title = "OBS Connection Error",
                    Content = message,
                    CloseButtonText = "Close",
                    MaxWidth = 400
                };

                uiMessageBox.Owner = System.Windows.Application.Current.MainWindow;

                await uiMessageBox.ShowDialogAsync();
            }
        });

        public string Address
        {
            get => _storage.Current.Obs.Address;
            set
            {
                if (_storage.Current.Obs.Address != value)
                {
                    _storage.Current.Obs.Address = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public int Port
        {
            get => _storage.Current.Obs.Port;
            set
            {
                if (_storage.Current.Obs.Port != value)
                {
                    _storage.Current.Obs.Port = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _storage.Current.Obs.Password ?? "";
            set
            {
                if (_storage.Current.Obs.Password != value)
                {
                    _storage.Current.Obs.Password = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoConnectOnStartup
        {
            get => _storage.Current.Obs.AutoConnectOnStartup;
            set
            {
                if (_storage.Current.Obs.AutoConnectOnStartup == value) return;
                _storage.Current.Obs.AutoConnectOnStartup = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public bool AutoReconnect
        {
            get => _storage.Current.Obs.AutoReconnect;
            set
            {
                if (_storage.Current.Obs.AutoReconnect == value) return;
                _storage.Current.Obs.AutoReconnect = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public int ReconnectDelay
        {
            get => _storage.Current.Obs.ReconnectDelay;
            set
            {
                if (_storage.Current.Obs.ReconnectDelay != value)
                {
                    _storage.Current.Obs.ReconnectDelay = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public int KeepAliveIntervalSeconds
        {
            get => _storage.Current.Obs.KeepAliveIntervalSeconds;
            set
            {
                if (_storage.Current.Obs.KeepAliveIntervalSeconds != value)
                {
                    _storage.Current.Obs.KeepAliveIntervalSeconds = value;
                    _storage.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool CanEditSettings => _obsService.ConnectionState == ConnectionStatus.NotConnected || _obsService.ConnectionState == ConnectionStatus.Failed;
    }
}