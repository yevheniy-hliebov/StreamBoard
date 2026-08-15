using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamTabula.Features.Home.Views.Controls;

public partial class HomeBanner : UserControl
{
    public HomeBanner() => InitializeComponent();

    public string AppName
    {
        get => (string)GetValue(AppNameProperty);
        set => SetValue(AppNameProperty, value);
    }
    public static readonly DependencyProperty AppNameProperty =
        DependencyProperty.Register(nameof(AppName), typeof(string), typeof(HomeBanner), new PropertyMetadata("StreamTabula"));

    public string CurrentVersion
    {
        get => (string)GetValue(CurrentVersionProperty);
        set => SetValue(CurrentVersionProperty, value);
    }
    public static readonly DependencyProperty CurrentVersionProperty =
        DependencyProperty.Register(nameof(CurrentVersion), typeof(string), typeof(HomeBanner), new PropertyMetadata(string.Empty));

    public ImageSource BannerImage
    {
        get => (ImageSource)GetValue(BannerImageProperty);
        set => SetValue(BannerImageProperty, value);
    }
    public static readonly DependencyProperty BannerImageProperty =
        DependencyProperty.Register(nameof(BannerImage), typeof(ImageSource), typeof(HomeBanner),
            new PropertyMetadata(null));

    public ImageSource LogoImage
    {
        get => (ImageSource)GetValue(LogoImageProperty);
        set => SetValue(LogoImageProperty, value);
    }
    public static readonly DependencyProperty LogoImageProperty =
        DependencyProperty.Register(nameof(LogoImage), typeof(ImageSource), typeof(HomeBanner),
            new PropertyMetadata(null));

    public string AuthorName
    {
        get => (string)GetValue(AuthorNameProperty);
        set => SetValue(AuthorNameProperty, value);
    }
    public static readonly DependencyProperty AuthorNameProperty =
        DependencyProperty.Register(nameof(AuthorName), typeof(string), typeof(HomeBanner), new PropertyMetadata("inkluznyk"));

    public string LinkText
    {
        get => (string)GetValue(LinkTextProperty);
        set => SetValue(LinkTextProperty, value);
    }
    public static readonly DependencyProperty LinkTextProperty =
        DependencyProperty.Register(nameof(LinkText), typeof(string), typeof(HomeBanner), new PropertyMetadata("github.com/yevheniy-hliebov/StreamTabula"));

    public string LinkUrl
    {
        get => (string)GetValue(LinkUrlProperty);
        set => SetValue(LinkUrlProperty, value);
    }
    public static readonly DependencyProperty LinkUrlProperty =
        DependencyProperty.Register(nameof(LinkUrl), typeof(string), typeof(HomeBanner), new PropertyMetadata("https://github.com/yevheniy-hliebov/StreamTabula"));

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(LinkUrl))
        {
            Process.Start(new ProcessStartInfo(LinkUrl)
            {
                UseShellExecute = true
            });
        }
    }
}