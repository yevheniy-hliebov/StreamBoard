using Microsoft.Win32;
using StreamBoard.Core;
using StreamBoard.Features.Settings.Services;
using Wpf.Ui.Appearance;

namespace StreamBoard.Features.Settings.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SettingsStorage _storage;
        private readonly StartupService _startupService;

        public SettingsViewModel(SettingsStorage storage, StartupService startupService)
        {
            _storage = storage;
            _startupService = startupService;
        }

        public bool IsDarkTheme
        {
            get => _storage.Current.Theme == "Dark";
            set
            {
                if (IsDarkTheme == value) return;
                _storage.Current.Theme = value ? "Dark" : "Light";
                _storage.Save();

                ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
                OnPropertyChanged();
            }
        }

        public bool MinimizeToTray
        {
            get => _storage.Current.MinimizeToTray;
            set
            {
                if (_storage.Current.MinimizeToTray == value) return;
                _storage.Current.MinimizeToTray = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public bool StartMinimized
        {
            get => _storage.Current.StartMinimized;
            set
            {
                if (_storage.Current.StartMinimized == value) return;
                _storage.Current.StartMinimized = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public bool StartupWithWindows
        {
            get => _storage.Current.StartupWithWindows;
            set
            {
                if (_storage.Current.StartupWithWindows == value) return;
                _storage.Current.StartupWithWindows = value;
                _storage.Save();

                _startupService.SetStartup(value);

                OnPropertyChanged();
            }
        }
    }
}
