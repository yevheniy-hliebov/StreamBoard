using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.Views.Components
{
    public partial class CategoryExpander : UserControl
    {
        public CategoryExpander() => InitializeComponent();

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CategoryExpander), new PropertyMetadata(string.Empty));

        public SymbolRegular IconSymbol
        {
            get => (SymbolRegular)GetValue(IconSymbolProperty);
            set => SetValue(IconSymbolProperty, value);
        }
        public static readonly DependencyProperty IconSymbolProperty =
            DependencyProperty.Register(nameof(IconSymbol), typeof(SymbolRegular), typeof(CategoryExpander), new PropertyMetadata(SymbolRegular.Folder24));

        public string? IconPath
        {
            get => (string?)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }
        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register(nameof(IconPath), typeof(string), typeof(CategoryExpander), new PropertyMetadata(null));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(CategoryExpander), new PropertyMetadata(true));

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }
        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(CategoryExpander), new PropertyMetadata(null));
    }
}
