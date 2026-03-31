using System.Reflection;
using System.Windows;
using Wpf.Ui.Controls;

namespace StreamBoard.Components.Controls
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IconGlyphAttribute(string glyph) : Attribute
    {
        public string Glyph { get; } = glyph;
    }

    public enum FluentIconType
    {
        [IconGlyph("\uE710")] Add,
        [IconGlyph("\uED35")] Apps,
        [IconGlyph("\uE74D")] Delete,
        [IconGlyph("\uEA6C")] Devices,
        [IconGlyph("\uE8A5")] Document,
        [IconGlyph("\uE739")] Checkbox,
        [IconGlyph("\uE70D")] ChevronDown,
        [IconGlyph("\uE76C")] ChevronRight,
        [IconGlyph("\uE8BB")] ChromeClose,
        [IconGlyph("\uED62")] ClearAllInk,
        [IconGlyph("\uE790")] Color,
        [IconGlyph("\uE9A6")] FitPage,
        [IconGlyph("\uE8B7")] Folder,
        [IconGlyph("\uE12B")] Globe,
        [IconGlyph("\uF0E2")] Grid,
        [IconGlyph("\uE76F")] GripperBarHorizontal,
        [IconGlyph("\uE897")] Help,
        [IconGlyph("\uE80F")] Home,
        [IconGlyph("\uE92E")] Keyboard,
        [IconGlyph("\uE72E")] Lock,
        [IconGlyph("\uE708")] Moon,
        [IconGlyph("\uE968")] Network,
        [IconGlyph("\uE91B")] Photo,
        [IconGlyph("\uE7E8")] PowerButton,
        [IconGlyph("\uEA86")] Puzzle,
        [IconGlyph("\uE8AC")] Rename,
        [IconGlyph("\uE71E")] Search,
        [IconGlyph("\uE713")] Settings,
        [IconGlyph("\uE71A")] Stop,
        [IconGlyph("\uE895")] Sync,
        [IconGlyph("\uE770")] System,
        [IconGlyph("\uE916")] Timer
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
            var field = type.GetType().GetField(type.ToString());
            var attribute = field?.GetCustomAttribute<IconGlyphAttribute>();

            this.Glyph = attribute?.Glyph ?? "\uE739";
        }
    }
}
