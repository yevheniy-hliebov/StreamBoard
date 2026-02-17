using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.Views.Components
{
    public partial class ActionLibraryItem : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(SymbolRegular), typeof(ActionLibraryItem),
                new PropertyMetadata(SymbolRegular.Empty));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ActionLibraryItem),
                new PropertyMetadata(string.Empty));

        public SymbolRegular Icon
        {
            get => (SymbolRegular)GetValue(IconProperty);
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