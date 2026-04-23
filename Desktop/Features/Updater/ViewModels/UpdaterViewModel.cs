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
                MessageBox.Show("Start downloading...", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}