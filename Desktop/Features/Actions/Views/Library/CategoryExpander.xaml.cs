using StreamTabula.Controls.Icons;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace StreamTabula.Features.Actions.Views.Library;

[ContentProperty(nameof(InnerContent))]
public partial class CategoryExpander : UserControl
{
    public CategoryExpander() => InitializeComponent();

    public string Category
    {
        get => (string)GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }
    public static readonly DependencyProperty CategoryProperty =
        DependencyProperty.Register("Category", typeof(string), typeof(CategoryExpander), new PropertyMetadata(string.Empty));

    public FluentIconType Icon
    {
        get => (FluentIconType)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(FluentIconType), typeof(CategoryExpander), new PropertyMetadata(FluentIconType.Folder));

    public IntegrationIconType? IntegrationIcon
    {
        get => (IntegrationIconType?)GetValue(IntegrationIconProperty);
        set => SetValue(IntegrationIconProperty, value);
    }
    public static readonly DependencyProperty IntegrationIconProperty =
        DependencyProperty.Register(nameof(IntegrationIcon), typeof(IntegrationIconType?), typeof(CategoryExpander), new PropertyMetadata(null));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CategoryExpander), new PropertyMetadata(true));

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }
    public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register("InnerContent", typeof(object), typeof(CategoryExpander), new PropertyMetadata(null));
}
