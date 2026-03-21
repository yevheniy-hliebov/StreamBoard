using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Components.TitleBar
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
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MainTitleBar), new PropertyMetadata("StreamBoard"));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MainTitleBar), new PropertyMetadata(null));
    }
}
