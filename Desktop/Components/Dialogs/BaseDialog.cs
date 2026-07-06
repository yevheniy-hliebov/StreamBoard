using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace StreamTabula.Components.Dialogs;

public class BaseDialog : FluentWindow
{
    public BaseDialog()
    {
        Background = (Brush)Application.Current.Resources["ApplicationBackgroundBrush"];
        Foreground = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

        Width = 350;
        MinWidth = 350;
        MaxWidth = 350;

        ExtendsContentIntoTitleBar = true;
        ShowInTaskbar = true;
        WindowBackdropType = WindowBackdropType.Mica;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Height;
    }

    protected void Submit(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    protected void Cancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
