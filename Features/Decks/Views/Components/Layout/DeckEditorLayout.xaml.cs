using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Decks.Views.Components.Layout
{
    public partial class DeckEditorLayout : UserControl
    {
        public DeckEditorLayout() => InitializeComponent();

        public object LeftBarContent
        {
            get => GetValue(LeftBarContentProperty);
            set => SetValue(LeftBarContentProperty, value);
        }
        public static readonly DependencyProperty LeftBarContentProperty =
            DependencyProperty.Register(nameof(LeftBarContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));
        
        public object CanvasContent
        {
            get => GetValue(CanvasContentProperty);
            set => SetValue(CanvasContentProperty, value);
        }
        public static readonly DependencyProperty CanvasContentProperty =
            DependencyProperty.Register(nameof(CanvasContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));

        public object PropertiesContent
        {
            get => GetValue(PropertiesContentProperty);
            set => SetValue(PropertiesContentProperty, value);
        }
        public static readonly DependencyProperty PropertiesContentProperty =
            DependencyProperty.Register(nameof(PropertiesContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));

        public object LibraryContent
        {
            get => GetValue(LibraryContentProperty);
            set => SetValue(LibraryContentProperty, value);
        }
        public static readonly DependencyProperty LibraryContentProperty =
            DependencyProperty.Register(nameof(LibraryContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));
    }
}
