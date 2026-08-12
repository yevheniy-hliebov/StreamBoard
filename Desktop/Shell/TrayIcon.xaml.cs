using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Shell;

public partial class TrayIconView : UserControl
{
    public TrayIconView() => InitializeComponent();

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        OpenApplication();
    }

    private void NotifyIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
    {
        OpenApplication();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private static void OpenApplication()
    {
        var mainWindow = Application.Current.MainWindow;
        if (mainWindow != null)
        {
            mainWindow.Show();
            if (mainWindow.WindowState == WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            mainWindow.Activate();
        }
    }
}