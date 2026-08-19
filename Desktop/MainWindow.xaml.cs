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
        IServiceProvider serviceProvider,
        ISnackbarService snackbarService,
        IntegrationsViewModel integrationsViewModel,
        SettingsStorage settings,
        AppInfoService appInfoService,
        StreamTabula.Features.Navigation.Services.NavigationService navService)
    {
        InitializeComponent();

        WindowBackdropType = WindowBackdropType.Mica;

        IntegrationsVM = integrationsViewModel;
        AppInfo = appInfoService.AppInfo;

        DataContext = this;

        snackbarService.SetSnackbarPresenter(RootSnackbarPresenter);

        _boundsManager = new WindowBoundsManager(this, settings);

        RootNavView.Initialize(serviceProvider, navService, settings);

        TextBoxFocusHelper.Attach(this);
    }
}