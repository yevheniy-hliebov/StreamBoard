using StreamTabula.Features.Settings.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Controls.Icons;

public partial class AdminStatusIcon : UserControl
{
    public AdminStatusIcon()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        bool isAdmin = AdminPrivilegeHelper.IsRunningAsAdministrator();
        if (isAdmin)
        {
            Visibility = Visibility.Visible;
        }
        else
        {
            Visibility = Visibility.Collapsed;
        }
    }
}