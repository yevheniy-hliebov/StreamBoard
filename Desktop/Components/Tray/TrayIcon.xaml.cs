using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Tray
{
    public partial class TrayIconView : UserControl
    {
        public TrayIconView()
        {
            InitializeComponent();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}