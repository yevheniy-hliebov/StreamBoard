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
            RestoreWindowBounds();

            if (_settings.Current.StartMinimized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void RestoreWindowBounds()
        {
            var config = _settings.Current;

            if (config.WindowWidth <= 0 || config.WindowHeight <= 0)
            {
                Width = SystemParameters.WorkArea.Width * 0.8;
                Height = SystemParameters.WorkArea.Height * 0.8;

                Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
                Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
            }
            else
            {
                Width = config.WindowWidth;
                Height = config.WindowHeight;
                Left = config.WindowLeft;
                Top = config.WindowTop;

                if (Left < SystemParameters.VirtualScreenLeft || Left >= SystemParameters.VirtualScreenWidth ||
                    Top < SystemParameters.VirtualScreenTop || Top >= SystemParameters.VirtualScreenHeight)
                {
                    Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
                    Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
                }

                if (config.IsWindowMaximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            SaveWindowBounds();
        }

        private void SaveWindowBounds()
        {
            var config = _settings.Current;

            if (WindowState == WindowState.Maximized)
            {
                config.IsWindowMaximized = true;
                config.WindowWidth = RestoreBounds.Width;
                config.WindowHeight = RestoreBounds.Height;
                config.WindowLeft = RestoreBounds.Left;
                config.WindowTop = RestoreBounds.Top;
            }
            else if (WindowState == WindowState.Normal)
            {
                config.IsWindowMaximized = false;
                config.WindowWidth = Width;
                config.WindowHeight = Height;
                config.WindowLeft = Left;
                config.WindowTop = Top;
            }
            else if (WindowState == WindowState.Minimized)
            {
                config.WindowWidth = RestoreBounds.Width;
                config.WindowHeight = RestoreBounds.Height;
                config.WindowLeft = RestoreBounds.Left;
                config.WindowTop = RestoreBounds.Top;
            }

             _settings.Save();
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