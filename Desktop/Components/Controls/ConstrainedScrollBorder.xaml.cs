using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace StreamTabula.Components.Controls
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


        public int ContentPadding
        {
            get { return (int)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register(nameof(ContentPadding), typeof(int), typeof(ConstrainedScrollBorder), new PropertyMetadata(0));
    }
}
