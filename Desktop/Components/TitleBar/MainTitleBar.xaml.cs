using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.TitleBar
{
    public partial class MainTitleBar : UserControl
    {
        public MainTitleBar() => InitializeComponent();

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MainTitleBar), new PropertyMetadata("StreamTabula"));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MainTitleBar), new PropertyMetadata(null));

        public object? RightContent
        {
            get => GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(nameof(RightContent), typeof(object), typeof(MainTitleBar), new PropertyMetadata(null));
    }
}
