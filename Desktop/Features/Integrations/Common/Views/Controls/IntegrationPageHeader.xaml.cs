using System.Windows;
using System.Windows.Controls;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Integrations.Common.Views.Controls;

public partial class IntegrationPageHeader : UserControl
{
    public IntegrationPageHeader() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(IntegrationPageHeader), new PropertyMetadata(string.Empty));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(IntegrationPageHeader), new PropertyMetadata(string.Empty));

    public IntegrationIconType IconType
    {
        get => (IntegrationIconType)GetValue(IconTypeProperty);
        set => SetValue(IconTypeProperty, value);
    }

    public static readonly DependencyProperty IconTypeProperty =
        DependencyProperty.Register(nameof(IconType), typeof(IntegrationIconType), typeof(IntegrationPageHeader), new PropertyMetadata(null));
}