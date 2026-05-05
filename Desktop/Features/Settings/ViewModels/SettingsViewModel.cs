using Microsoft.Win32;
using StreamTabula.Core;
using StreamTabula.Features.Navigation.Services;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Updater.ViewModels;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Settings.ViewModels
{
    public class SettingsViewModel(
        SettingsStorage storage,
        StartupService startupService,
        PrivilegeService privilegeService,
        NavigationService pageService,
        UpdaterViewModel updaterViewModel
    ) : ObservableObject
    {
        private readonly SettingsStorage _storage = storage;
        private readonly StartupService _startupService = startupService;
        private readonly PrivilegeService _privilegeService = privilegeService;
        private readonly UpdaterViewModel _updaterViewModel = updaterViewModel;

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

        public bool RunAsAdmin
        {
            get => _storage.Current.RunAsAdmin;
            set
            {
                if (_storage.Current.RunAsAdmin == value) return;
                _storage.Current.RunAsAdmin = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public bool IsRunAsAdmin => _privilegeService.IsRunAsAdmin();

        public ICommand RestartAsAdminCommand => new RelayCommand<object>(_ => _privilegeService.RestartAsAdmin());

        public List<string> AvailableStartupPages { get; } = pageService.AllPages.Select(p => p.Name).ToList();

        public string StartupPage
        {
            get => _storage.Current.StartupPage;
            set
            {
                if (_storage.Current.StartupPage == value) return;
                _storage.Current.StartupPage = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public List<string> AvailableUpdateChannels { get; } = ["Stable releases", "Beta releases"];

        public string UpdateChannel
        {
            get => _storage.Current.UpdateChannel;
            set
            {
                if (_storage.Current.UpdateChannel == value) return;
                _storage.Current.UpdateChannel = value;
                _storage.Save();
                OnPropertyChanged();
            }
        }

        public string CurrentVersion => $"Current version: v{_updaterViewModel.AppInfo.CurrentVersion}";

        public ICommand CheckForUpdatesCommand => _updaterViewModel.OpenUpdateDialogCommand;
    }
}
