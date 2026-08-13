using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Controls.Typography;

public partial class PageTitle : UserControl
{
    public PageTitle() => InitializeComponent();

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(PageTitle), new PropertyMetadata("Page Title"));
}
