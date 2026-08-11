using System.Reflection;
using System.Windows;
using StreamTabula.Components.Enums;
using Wpf.Ui.Controls;

namespace StreamTabula.Components.Controls
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IconGlyphAttribute(string glyph) : Attribute
    {
        public string Glyph { get; } = glyph;
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
