using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.Views.Components.Canvas
{
    public partial class DeckButtonSlotView : UserControl
    {
        public DeckButtonSlotView() => InitializeComponent();

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DeckButtonSlotView), new PropertyMetadata(false));


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DeckButtonSlotView), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(DeckButtonSlotView), new PropertyMetadata(null));


        public string ButtonName
        {
            get { return (string)GetValue(ButtonNameProperty); }
            set { SetValue(ButtonNameProperty, value); }
        }

        public static readonly DependencyProperty ButtonNameProperty =
            DependencyProperty.Register(nameof(ButtonName), typeof(string), typeof(DeckButtonSlotView), new PropertyMetadata(""));


        public int ButtonIndex
        {
            get { return (int)GetValue(ButtonIndexProperty); }
            set { SetValue(ButtonIndexProperty, value); }
        }

        public static readonly DependencyProperty ButtonIndexProperty =
            DependencyProperty.Register(nameof(ButtonIndex), typeof(int), typeof(DeckButtonSlotView), new PropertyMetadata(0));


        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(DeckButtonSlotView), new PropertyMetadata(null));


        public string ButtonBackground
        {
            get { return (string)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonBackground), typeof(string), typeof(DeckButtonSlotView), new PropertyMetadata("#2B2B2B"));

    }
}
