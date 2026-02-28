using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Decks.Views.Components.Layout
{
    public partial class DeckSidebarHeader : UserControl
    {
        public DeckSidebarHeader() => InitializeComponent();

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(DeckSidebarHeader),
                new PropertyMetadata("")
            );

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register(
                nameof(RightContent),
                typeof(object),
                typeof(DeckSidebarHeader),
                new PropertyMetadata(null)
            );

        public object RightContent
        {
            get => GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        public static readonly DependencyProperty BottomContentProperty =
            DependencyProperty.Register(
                nameof(BottomContent),
                typeof(object),
                typeof(DeckSidebarHeader),
                new PropertyMetadata(null)
            );

        public object BottomContent
        {
            get => GetValue(BottomContentProperty);
            set => SetValue(BottomContentProperty, value);
        }
    }
}
