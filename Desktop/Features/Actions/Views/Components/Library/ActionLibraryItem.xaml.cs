using StreamBoard.Core.Models;
using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Features.Actions.Views.Components.Library
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