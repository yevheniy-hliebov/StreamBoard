using System.Windows;
using Wpf.Ui.Controls;

namespace StreamBoard.Components.Controls
{
    public enum FluentIconType
    {
        Add,
        Rename,
        Delete,
        Home,
        Grid,
        Keyboard,
        Devices,
        Network,
        Globe,
        System,
        Settings
    }

    public partial class FluentIcon : FontIcon
    {
        public FluentIcon()
        {
            InitializeComponent();

            this.Glyph = "\uE710";
        }

        public static readonly DependencyProperty IconTypeProperty =
        DependencyProperty.Register(nameof(IconType), typeof(FluentIconType), typeof(FluentIcon),
            new PropertyMetadata(FluentIconType.Add, OnIconTypeChanged));

        public FluentIconType IconType
        {
            get => (FluentIconType)GetValue(IconTypeProperty);
            set => SetValue(IconTypeProperty, value);
        }

        private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FluentIcon control && e.NewValue is FluentIconType type)
            {
                control.Glyph = type switch
                {
                    FluentIconType.Add => "\uE710",
                    FluentIconType.Rename => "\uE8AC",
                    FluentIconType.Delete => "\uE74D",
                    FluentIconType.Home => "\uE80F",
                    FluentIconType.Grid => "\uF0E2",
                    FluentIconType.Keyboard => "\uE92E",
                    FluentIconType.Devices => "\uEA6C",
                    FluentIconType.Network => "\uE968",
                    FluentIconType.Globe => "\uE12B",
                    FluentIconType.System => "\uE770",
                    FluentIconType.Settings => "\uE713",
                    _ => "\uE710"
                };
            }
        }
    }
}
