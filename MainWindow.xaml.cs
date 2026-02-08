using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Settings.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace StreamBoard
{
    public partial class MainWindow : FluentWindow
    {
        private readonly SettingsStorage _settings;

        public MainWindow()
        {
            InitializeComponent();

            WindowBackdropType = Wpf.Ui.Controls.WindowBackdropType.Mica;

            _settings = App.ServiceProvider.GetRequiredService<SettingsStorage>();

            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (_settings.Current.MinimizeToTray && WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }
    }
}