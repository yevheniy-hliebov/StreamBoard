using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Settings.Views.Controls;

public partial class PrivilegeStatusInfo : UserControl
{
    public PrivilegeStatusInfo() => InitializeComponent();

    public bool IsRunAsAdmin
    {
        get { return (bool)GetValue(IsRunAsAdminProperty); }
        set { SetValue(IsRunAsAdminProperty, value); }
    }

    public static readonly DependencyProperty IsRunAsAdminProperty =
        DependencyProperty.Register(nameof(IsRunAsAdmin), typeof(bool), typeof(PrivilegeStatusInfo), new PropertyMetadata(false));

    public ICommand RestartAsAdminCommand
    {
        get { return (ICommand)GetValue(RestartAsAdminCommandProperty); }
        set { SetValue(RestartAsAdminCommandProperty, value); }
    }

    public static readonly DependencyProperty RestartAsAdminCommandProperty =
        DependencyProperty.Register(nameof(RestartAsAdminCommand), typeof(ICommand), typeof(PrivilegeStatusInfo), new PropertyMetadata(null));
}
