using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Components.Cards
{
    public enum IconPosition { Top, Left }

    public partial class NavigationCard : UserControl
    {
        public NavigationCard()
        {
            InitializeComponent();
        }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(NavigationCard), new PropertyMetadata("Title"));

        public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(NavigationCard), new PropertyMetadata(""));

        public object Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(NavigationCard));

        public bool IsChevronVisible { get => (bool)GetValue(IsChevronVisibleProperty); set => SetValue(IsChevronVisibleProperty, value); }
        public static readonly DependencyProperty IsChevronVisibleProperty = DependencyProperty.Register(nameof(IsChevronVisible), typeof(bool), typeof(NavigationCard), new PropertyMetadata(false));

        public IconPosition IconPosition { get => (IconPosition)GetValue(IconPositionProperty); set => SetValue(IconPositionProperty, value); }
        public static readonly DependencyProperty IconPositionProperty = DependencyProperty.Register(nameof(IconPosition), typeof(IconPosition), typeof(NavigationCard), new PropertyMetadata(IconPosition.Top));

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(NavigationCard));
    }
}
