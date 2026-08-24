using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Integrations.Twitch.Views.Controls;

public partial class TwitchLoginCard : UserControl
{
    public TwitchLoginCard() => InitializeComponent();

    public string AccountRole
    {
        get => (string)GetValue(AccountRoleProperty);
        set => SetValue(AccountRoleProperty, value);
    }

    public static readonly DependencyProperty AccountRoleProperty =
        DependencyProperty.Register(nameof(AccountRole), typeof(string), typeof(TwitchLoginCard), new PropertyMetadata("AccountRole"));

    public ICommand LoginCommand
    {
        get => (ICommand)GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }

    public static readonly DependencyProperty LoginCommandProperty =
        DependencyProperty.Register(nameof(LoginCommand), typeof(ICommand), typeof(TwitchLoginCard));
}