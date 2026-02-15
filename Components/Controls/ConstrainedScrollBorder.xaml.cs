using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace StreamBoard.Components.Controls
{
    [ContentProperty(nameof(InnerContent))]
    public partial class ConstrainedScrollBorder : UserControl
    {
        public ConstrainedScrollBorder() => InitializeComponent();

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(ConstrainedScrollBorder), new PropertyMetadata(null));
    }
}
