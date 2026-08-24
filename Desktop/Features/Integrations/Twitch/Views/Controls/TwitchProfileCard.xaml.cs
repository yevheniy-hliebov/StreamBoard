using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Views.Controls;

public partial class TwitchProfileCard : UserControl
{
    public TwitchProfileCard() => InitializeComponent();

    public TwitchUserIdentity? User
    {
        get => (TwitchUserIdentity?)GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public static readonly DependencyProperty UserProperty =
        DependencyProperty.Register(nameof(User), typeof(TwitchUserIdentity), typeof(TwitchProfileCard), new PropertyMetadata(null));

    public string AccountRole
    {
        get => (string)GetValue(AccountRoleProperty);
        set => SetValue(AccountRoleProperty, value);
    }

    public static readonly DependencyProperty AccountRoleProperty =
        DependencyProperty.Register(nameof(AccountRole), typeof(string), typeof(TwitchProfileCard), new PropertyMetadata("AccountRole"));

    public ICommand LogoutCommand
    {
        get => (ICommand)GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public static readonly DependencyProperty LogoutCommandProperty =
        DependencyProperty.Register(nameof(LogoutCommand), typeof(ICommand), typeof(TwitchProfileCard));
}