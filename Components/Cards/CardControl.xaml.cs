using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Components.Cards
{
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CardControl), new PropertyMetadata("Title"));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CardControl), new PropertyMetadata("Description"));

        public object ActionContent
        {
            get => GetValue(ActionContentProperty);
            set => SetValue(ActionContentProperty, value);
        }
        public static readonly DependencyProperty ActionContentProperty =
            DependencyProperty.Register("ActionContent", typeof(object), typeof(CardControl), new PropertyMetadata(null));
    }
}
