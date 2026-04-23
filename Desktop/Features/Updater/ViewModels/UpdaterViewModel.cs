using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using StreamBoard.Core;
using StreamBoard.Features.Settings.Services;
using StreamBoard.Features.Updater.Models;
using StreamBoard.Features.Updater.Services;
using StreamBoard.Features.Updater.Views.Components;

namespace StreamBoard.Features.Updater.ViewModels
{
    public partial class UpdaterViewModel : ObservableObject
    {
        private readonly AppInfoService _appInfoService;
        private readonly UpdateService _updateService;
        private readonly SettingsStorage _settingsStorage;

        public ICommand OpenUpdateDialogCommand { get; }
        public ICommand UpdateNowCommand { get; }
        public ICommand SkipVersionCommand { get; }

        public UpdaterViewModel(AppInfoService appInfoService, UpdateService updateService, SettingsStorage settingsStorage)
        {
            _appInfoService = appInfoService;
            _updateService = updateService;
            _settingsStorage = settingsStorage;

            OpenUpdateDialogCommand = new RelayCommand(_ => OpenUpdateDialog());

            UpdateNowCommand = new RelayCommand(_ =>
            {
                _ = StartUpdateProcessAsync();
            });

            SkipVersionCommand = new RelayCommand(_ =>
            {
                if (LatestReleaseInfo != null)
                {
                    _settingsStorage.Current.SkippedVersion = LatestReleaseInfo.TagName;
                    _settingsStorage.Save();
                }
            });
        }

        public AppInfoModel AppInfo => _appInfoService.AppInfo;

        private string _dialogState = "Loading";
        public string DialogState
        {
            get => _dialogState;
            set => SetProperty(ref _dialogState, value);
        }

        private GithubReleaseInfo? _latestReleaseInfo;
        public GithubReleaseInfo? LatestReleaseInfo
        {
            get => _latestReleaseInfo;
            set => SetProperty(ref _latestReleaseInfo, value);
        }

        private double _downloadProgress;
        public double DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }

        private string _downloadStatusText = "Preparing to download...";
        public string DownloadStatusText
        {
            get => _downloadStatusText;
            set => SetProperty(ref _downloadStatusText, value);
        }

        private void OpenUpdateDialog()
        {
            DialogState = "Loading";
            LatestReleaseInfo = null;

            _ = PerformUpdateCheckAsync();

            var updateDialog = new UpdateDialogWindow
            {
                DataContext = this,
                Owner = Application.Current.MainWindow
            };

            updateDialog.ShowDialog();
        }

        private async Task PerformUpdateCheckAsync()
        {
            await Task.Delay(600);

            var receiveBetaUpdates = _settingsStorage.Current.UpdateChannel == "Beta releases";
            var releaseInfo = await _updateService.CheckForUpdatesAsync(AppInfo, receiveBetaUpdates);

            LatestReleaseInfo = releaseInfo;

            if (releaseInfo != null)
            {
                DialogState = "UpdateAvailable";
            }
            else
            {
                DialogState = "UpToDate";
            }
        }

        public async Task CheckForUpdatesOnStartupAsync()
        {
            try
            {
                var receiveBetaUpdates = _settingsStorage.Current.UpdateChannel == "Beta releases";
                var releaseInfo = await _updateService.CheckForUpdatesAsync(AppInfo, receiveBetaUpdates);

                if (releaseInfo != null)
                {
                    if (_settingsStorage.Current.SkippedVersion == releaseInfo.TagName)
                    {
                        return;
                    }

                    LatestReleaseInfo = releaseInfo;
                    DialogState = "UpdateAvailable";

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var updateDialog = new UpdateDialogWindow
                        {
                            DataContext = this
                        };

                        var mainWindow = Application.Current.MainWindow;
                        if (mainWindow != null && mainWindow.IsVisible)
                        {
                            updateDialog.Owner = mainWindow;
                            updateDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        }
                        else
                        {
                            updateDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            updateDialog.Topmost = true;
                        }

                        updateDialog.ShowDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Startup update check failed: {ex.Message}");
            }
        }

        private async Task StartUpdateProcessAsync()
        {
            if (LatestReleaseInfo == null) return;

            DialogState = "Downloading";

            var progressIndicator = new Progress<double>(percent =>
            {
                DownloadProgress = percent;
                DownloadStatusText = "Downloading update archive...";
            });

            try
            {
                // Викликаємо наш новий метод
                string downloadedFilePath = await _updateService.DownloadUpdateArchiveAsync(LatestReleaseInfo, AppInfo, progressIndicator);

                // Для тесту можемо просто показати, куди він завантажився
                MessageBox.Show($"File downloaded successfully to:\n{downloadedFilePath}");

                // Тут пізніше буде виклик методу розпакування та бекапу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download update: {ex.Message}");
                DialogState = "UpdateAvailable";
            }
        }
    }
}