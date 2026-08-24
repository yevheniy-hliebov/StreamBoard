using StreamTabula.Features.Integrations.Twitch.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Integrations.Twitch.Views.Controls;

public partial class TwitchAccountCard : UserControl
{
    public TwitchAccountCard() => InitializeComponent();

    public string AccountRole
    {
        get { return (string)GetValue(AccountRoleProperty); }
        set { SetValue(AccountRoleProperty, value); }
    }

    public static readonly DependencyProperty AccountRoleProperty =
        DependencyProperty.Register(nameof(AccountRole), typeof(string), typeof(TwitchAccountCard), new PropertyMetadata("Account role"));

    public bool IsAuth
    {
        get { return (bool)GetValue(IsAuthProperty); }
        set { SetValue(IsAuthProperty, value); }
    }

    public static readonly DependencyProperty IsAuthProperty =
        DependencyProperty.Register(nameof(IsAuth), typeof(bool), typeof(TwitchAccountCard), new PropertyMetadata(false));

    public TwitchUserIdentity? User
    {
        get { return (TwitchUserIdentity?)GetValue(UserProperty); }
        set { SetValue(UserProperty, value); }
    }

    public static readonly DependencyProperty UserProperty =
        DependencyProperty.Register(nameof(User), typeof(TwitchUserIdentity), typeof(TwitchAccountCard), new PropertyMetadata(null));

    public ICommand LoginCommand
    {
        get => (ICommand)GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }

    public static readonly DependencyProperty LoginCommandProperty =
        DependencyProperty.Register(nameof(LoginCommand), typeof(ICommand), typeof(TwitchAccountCard));

    public ICommand LogoutCommand
    {
        get => (ICommand)GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public static readonly DependencyProperty LogoutCommandProperty =
        DependencyProperty.Register(nameof(LogoutCommand), typeof(ICommand), typeof(TwitchAccountCard));
}