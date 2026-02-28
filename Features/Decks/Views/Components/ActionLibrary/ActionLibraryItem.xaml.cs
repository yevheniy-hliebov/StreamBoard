using StreamBoard.Components.Controls;
using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Decks.Views.Components.ActionLibrary
{
    public partial class ActionLibraryItem : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(FluentIconType), typeof(ActionLibraryItem),
                new PropertyMetadata(FluentIconType.Checkbox));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ActionLibraryItem),
                new PropertyMetadata(string.Empty));

        public FluentIconType Icon
        {
            get => (FluentIconType)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ActionLibraryItem()
        {
            InitializeComponent();
        }
    }
}