using System.Windows;
using System.Windows.Controls;

namespace StreamBoard.Components.Cards
{
    public partial class CardWithToggleSwitch : UserControl
    {
        public CardWithToggleSwitch()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CardWithToggleSwitch), new PropertyMetadata("Title"));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CardWithToggleSwitch), new PropertyMetadata("Description goes here"));

        public string ToggleOffText
        {
            get { return (string)GetValue(ToggleOffTextProperty); }
            set { SetValue(ToggleOffTextProperty, value); }
        }
        public static readonly DependencyProperty ToggleOffTextProperty =
            DependencyProperty.Register("ToggleOffText", typeof(string), typeof(CardWithToggleSwitch), new PropertyMetadata("Off"));

        public string ToggleOnText
        {
            get { return (string)GetValue(ToggleOnTextProperty); }
            set { SetValue(ToggleOnTextProperty, value); }
        }
        public static readonly DependencyProperty ToggleOnTextProperty =
            DependencyProperty.Register("ToggleOnText", typeof(string), typeof(CardWithToggleSwitch), new PropertyMetadata("On"));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CardWithToggleSwitch),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}