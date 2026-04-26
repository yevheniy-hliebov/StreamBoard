using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamBoard.Components.Controls
{
    public class ClippingBorder : Border
    {
        private readonly RectangleGeometry _clip = new();
        private object? _oldClip;

        protected override void OnRender(DrawingContext dc)
        {
            ApplyChildClip();
            base.OnRender(dc);
        }

        public override UIElement? Child
        {
            get => base.Child;
            set
            {
                if (base.Child != value)
                {
                    base.Child?.SetValue(UIElement.ClipProperty, _oldClip);

                    _oldClip = value?.ReadLocalValue(UIElement.ClipProperty);
                    base.Child = value;
                }
            }
        }

        private void ApplyChildClip()
        {
            if (Child is null)
                return;

            double radius = Math.Max(0.0, CornerRadius.TopLeft - (BorderThickness.Left * 0.5));

            _clip.Rect = new Rect(Child.RenderSize);
            _clip.RadiusX = radius;
            _clip.RadiusY = radius;

            Child.Clip = _clip;
        }
    }
}