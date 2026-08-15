using StreamTabula.Features.Integrations.Common.ViewModels;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Updater.Models;
using StreamTabula.Features.Updater.Services;
using StreamTabula.Shell.Behaviors;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace StreamTabula;

public partial class MainWindow : FluentWindow
{
    public IntegrationsViewModel IntegrationsVM { get; }
    public AppInfoModel? AppInfo { get; }
    public string AppVersion => AppInfo != null ? $"(v{AppInfo.CurrentVersion})" : string.Empty;

    private readonly WindowBoundsManager _boundsManager;

    public MainWindow(
        ISnackbarService snackbarService,
        IntegrationsViewModel integrationsViewModel,
        SettingsStorage settings,
        AppInfoService appInfoService)
    {
        InitializeComponent();

        WindowBackdropType = WindowBackdropType.Mica;

        IntegrationsVM = integrationsViewModel;
        AppInfo = appInfoService.AppInfo;

        DataContext = this;

        snackbarService.SetSnackbarPresenter(RootSnackbarPresenter);

        _boundsManager = new WindowBoundsManager(this, settings);
        TextBoxFocusHelper.Attach(this);
    }
}