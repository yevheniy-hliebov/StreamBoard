using System.Windows;
using Wpf.Ui.Controls;

namespace StreamBoard.Components.Controls
{
    public enum FluentIconType
    {
        Add,
        Apps,
        Delete,
        Devices,
        Document,
        Checkbox,
        ChevronDown,
        ClearAllInk,
        Color,
        Folder,
        Globe,
        Grid,
        GripperBarHorizontal,
        Help,
        Home,
        Keyboard,
        Network,
        Photo,
        PowerButton,
        Rename,
        Search,
        Settings,
        Stop,
        Sync,
        System,
        Timer,
    }

    public partial class FluentIcon : FontIcon
    {
        static FluentIcon()
        {
            FontSizeProperty.OverrideMetadata(typeof(FluentIcon), new FrameworkPropertyMetadata(14.0));
        }

        public FluentIcon()
        {
            InitializeComponent();

            UpdateGlyph(IconType);
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
                control.UpdateGlyph(type);
            }
        }

        private void UpdateGlyph(FluentIconType type)
        {
            this.Glyph = type switch
            {
                FluentIconType.Add => "\uE710",
                FluentIconType.Apps => "\uED35",
                FluentIconType.Delete => "\uE74D",
                FluentIconType.Devices => "\uEA6C",
                FluentIconType.Document => "\uE8A5",
                FluentIconType.Checkbox => "\uE739",
                FluentIconType.ChevronDown => "\uE70D",
                FluentIconType.ClearAllInk => "\uED62",
                FluentIconType.Color => "\uE790",
                FluentIconType.Folder => "\uE8B7",
                FluentIconType.Globe => "\uE12B",
                FluentIconType.Grid => "\uF0E2",
                FluentIconType.GripperBarHorizontal => "\uE76F",
                FluentIconType.Help => "\uE897",
                FluentIconType.Home => "\uE80F",
                FluentIconType.Keyboard => "\uE92E",
                FluentIconType.Network => "\uE968",
                FluentIconType.Photo => "\uE91B",
                FluentIconType.PowerButton => "\uE7E8",
                FluentIconType.Rename => "\uE8AC",
                FluentIconType.Search => "\uE71E",
                FluentIconType.Settings => "\uE713",
                FluentIconType.Stop => "\uE71A",
                FluentIconType.Sync => "\uE895",
                FluentIconType.System => "\uE770",
                FluentIconType.Timer => "\uE916",
                _ => "\uE710"
            };
        }
    }
}
