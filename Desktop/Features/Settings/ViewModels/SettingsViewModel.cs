using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Navigation.Services;
using StreamTabula.Features.Settings.Helpers;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Updater.ViewModels;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Settings.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly SettingsStorage storage;
    private readonly UpdaterViewModel updaterViewModel;
    public IRelayCommand<object> RestartAsAdminCommand { get; }

    public SettingsViewModel(
        SettingsStorage storage,
        NavigationService pageService,
        UpdaterViewModel updaterViewModel)
    {
        this.storage = storage;
        this.updaterViewModel = updaterViewModel;
        AvailableStartupPages = pageService.AllPages.Select(p => p.Name).ToList();

        RestartAsAdminCommand = new RelayCommand<object>(_ => AdminPrivilegeHelper.RestartAsAdministrator());
    }

    public List<string> AvailableStartupPages { get; }

    public List<string> AvailableUpdateChannels { get; } = ["Stable releases", "Beta releases"];

    public bool IsDarkTheme
    {
        get => storage.Current.Appearance.Theme == "Dark";
        set
        {
            if (IsDarkTheme == value) return;
            storage.Current.Appearance.Theme = value ? "Dark" : "Light";
            storage.Save();

            ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTray
    {
        get => storage.Current.Startup.MinimizeToTray;
        set
        {
            if (storage.Current.Startup.MinimizeToTray == value) return;
            storage.Current.Startup.MinimizeToTray = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool StartMinimized
    {
        get => storage.Current.Startup.StartMinimized;
        set
        {
            if (storage.Current.Startup.StartMinimized == value) return;
            storage.Current.Startup.StartMinimized = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool StartupWithWindows
    {
        get => storage.Current.Startup.StartupWithWindows;
        set
        {
            if (storage.Current.Startup.StartupWithWindows == value) return;
            storage.Current.Startup.StartupWithWindows = value;
            storage.Save();

            _ = WindowsStartupHelper.SetStartWithWindowsAsync(updaterViewModel.AppInfo.AppName, value);

            OnPropertyChanged();
        }
    }

    public bool RunAsAdmin
    {
        get => storage.Current.Startup.RunAsAdmin;
        set
        {
            if (storage.Current.Startup.RunAsAdmin == value) return;
            storage.Current.Startup.RunAsAdmin = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool IsRunAsAdmin => AdminPrivilegeHelper.IsRunningAsAdministrator();

    public string StartupPage
    {
        get => storage.Current.Startup.StartupPage;
        set
        {
            if (storage.Current.Startup.StartupPage == value) return;
            storage.Current.Startup.StartupPage = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public string UpdateChannel
    {
        get => storage.Current.Updates.UpdateChannel;
        set
        {
            if (storage.Current.Updates.UpdateChannel == value) return;
            storage.Current.Updates.UpdateChannel = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public string CurrentVersion => $"Current version: v{updaterViewModel.AppInfo.CurrentVersion}";

    public ICommand CheckForUpdatesCommand => updaterViewModel.OpenUpdateDialogCommand;
}