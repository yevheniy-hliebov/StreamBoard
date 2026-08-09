using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Features.Integrations.Twitch.Views.Components;

public partial class TwitchRoundAvatar : UserControl
{
    public TwitchRoundAvatar()
    {
        InitializeComponent();
    }

    public string ImageUrl
    {
        get { return (string)GetValue(ImageUrlProperty); }
        set { SetValue(ImageUrlProperty, value); }
    }

    public static readonly DependencyProperty ImageUrlProperty =
        DependencyProperty.Register(
            nameof(ImageUrl),
            typeof(string),
            typeof(TwitchRoundAvatar),
            new PropertyMetadata(null));
}
