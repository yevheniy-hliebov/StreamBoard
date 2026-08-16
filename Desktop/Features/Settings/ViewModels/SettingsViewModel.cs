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
        get => storage.Current.Theme == "Dark";
        set
        {
            if (IsDarkTheme == value) return;
            storage.Current.Theme = value ? "Dark" : "Light";
            storage.Save();

            ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTray
    {
        get => storage.Current.MinimizeToTray;
        set
        {
            if (storage.Current.MinimizeToTray == value) return;
            storage.Current.MinimizeToTray = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool StartMinimized
    {
        get => storage.Current.StartMinimized;
        set
        {
            if (storage.Current.StartMinimized == value) return;
            storage.Current.StartMinimized = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool StartupWithWindows
    {
        get => storage.Current.StartupWithWindows;
        set
        {
            if (storage.Current.StartupWithWindows == value) return;
            storage.Current.StartupWithWindows = value;
            storage.Save();

            _ = WindowsStartupHelper.SetStartWithWindowsAsync(updaterViewModel.AppInfo.AppName, value);

            OnPropertyChanged();
        }
    }

    public bool RunAsAdmin
    {
        get => storage.Current.RunAsAdmin;
        set
        {
            if (storage.Current.RunAsAdmin == value) return;
            storage.Current.RunAsAdmin = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public bool IsRunAsAdmin => AdminPrivilegeHelper.IsRunningAsAdministrator();

    public string StartupPage
    {
        get => storage.Current.StartupPage;
        set
        {
            if (storage.Current.StartupPage == value) return;
            storage.Current.StartupPage = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public string UpdateChannel
    {
        get => storage.Current.UpdateChannel;
        set
        {
            if (storage.Current.UpdateChannel == value) return;
            storage.Current.UpdateChannel = value;
            storage.Save();
            OnPropertyChanged();
        }
    }

    public string CurrentVersion => $"Current version: v{updaterViewModel.AppInfo.CurrentVersion}";

    public ICommand CheckForUpdatesCommand => updaterViewModel.OpenUpdateDialogCommand;
}