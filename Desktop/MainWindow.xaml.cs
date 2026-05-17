using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Integrations.Common.ViewModels;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Updater.Models;
using StreamTabula.Features.Updater.Services;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace StreamTabula
{
    public partial class MainWindow : FluentWindow
    {
        private readonly SettingsStorage _settings;
        private readonly IntegrationsViewModel _integrationsViewModel;

        public AppInfoModel? AppInfo { get; }

        public string AppVersion => AppInfo != null ? $"(v{AppInfo.CurrentVersion})" : string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            WindowBackdropType = WindowBackdropType.Mica;

            var snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
            snackbarService.SetSnackbarPresenter(RootSnackbarPresenter);

            _integrationsViewModel = App.ServiceProvider.GetRequiredService<IntegrationsViewModel>();

            _settings = App.ServiceProvider.GetRequiredService<SettingsStorage>();

            var appInfoService = App.ServiceProvider.GetRequiredService<AppInfoService>();

            AppInfo = appInfoService.AppInfo;

            Loaded += MainWindow_Loaded;
            StateChanged += MainWindow_StateChanged;

            this.DataContext = _integrationsViewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_settings.Current.StartMinimized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (_settings.Current.MinimizeToTray && WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (Keyboard.FocusedElement is System.Windows.Controls.Primitives.TextBoxBase focusedTextBox)
            {
                DependencyObject? clickedElement = e.OriginalSource as DependencyObject;
                bool clickedInsideAnyTextBox = false;
                var current = clickedElement;

                while (current != null)
                {
                    if (current is System.Windows.Controls.Primitives.TextBoxBase)
                    {
                        clickedInsideAnyTextBox = true;
                        break;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }

                if (!clickedInsideAnyTextBox)
                {
                    Keyboard.ClearFocus();

                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(focusedTextBox), null);

                    this.Focus();
                }
            }
        }
    }
}