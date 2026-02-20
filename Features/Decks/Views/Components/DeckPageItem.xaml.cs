using StreamBoard.Features.Decks.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StreamBoard.Features.Decks.Views.Components
{
    public partial class DeckPageItem : UserControl
    {
        public DeckPageItem()
        {
            InitializeComponent();

            EditBox.KeyDown += OnEditBoxKeyDown;

            EditBox.IsVisibleChanged += (s, e) =>
            {
                if ((bool)e.NewValue)
                {
                    EditBox.Focus();
                    EditBox.SelectAll();
                }
            };
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(DeckPageItem), new PropertyMetadata("Page"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private void OnEditBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                var parentPage = FindParent<Page>(this);

                if (parentPage?.DataContext is GridDeckViewModel vm)
                {
                    vm.Pages.EndRename();

                    Keyboard.ClearFocus();

                    e.Handled = true;
                }
            }
        }

        private T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent) return parent;

            return FindParent<T>(parentObject);
        }
    }
}
