using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StreamTabula.Core.Models;

namespace StreamTabula.Components.Controls
{
    public partial class IconButton : UserControl
    {
        public IconButton() => InitializeComponent();

        public string TooltipText
        {
            get { return (string)GetValue(TooltipTextProperty); }
            set { SetValue(TooltipTextProperty, value); }
        }

        public static readonly DependencyProperty TooltipTextProperty =
            DependencyProperty.Register(nameof(TooltipText), typeof(string), typeof(IconButton), new PropertyMetadata(string.Empty));


        public FluentIconType IconType
        {
            get { return (FluentIconType)GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }

        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register(nameof(IconType), typeof(FluentIconType), typeof(IconButton), new PropertyMetadata(FluentIconType.Checkbox));


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(IconButton));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(IconButton), new PropertyMetadata(null));
    }
}
