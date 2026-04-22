using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.Views.Components.DeckProfile
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

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(DeckPageItem), new PropertyMetadata(false));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly DependencyProperty EndEditCommandProperty =
            DependencyProperty.Register(nameof(EndEditCommand), typeof(ICommand), typeof(DeckPageItem));

        public ICommand EndEditCommand
        {
            get => (ICommand)GetValue(EndEditCommandProperty);
            set => SetValue(EndEditCommandProperty, value);
        }

        private void OnEditBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                if (EndEditCommand?.CanExecute(null) == true)
                {
                    EndEditCommand.Execute(null);
                }

                Keyboard.ClearFocus();
                e.Handled = true;
            }
        }
    }
}