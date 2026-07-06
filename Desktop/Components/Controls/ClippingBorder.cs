using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamTabula.Components.Controls
{
    public class ClippingBorder : Border
    {
        private RectangleGeometry? _clip;

        public override UIElement? Child
        {
            get => base.Child;
            set
            {
                if (base.Child != value)
                {
                    base.Child?.Clip = null;

                    base.Child = value;

                    ApplyChildClip();
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ApplyChildClip();
        }

        private void ApplyChildClip()
        {
            if (Child is null)
                return;

            if (_clip == null)
            {
                _clip = new RectangleGeometry();
                Child.Clip = _clip;
            }

            double width = Math.Max(0, this.RenderSize.Width - this.BorderThickness.Left - this.BorderThickness.Right - this.Padding.Left - this.Padding.Right);
            double height = Math.Max(0, this.RenderSize.Height - this.BorderThickness.Top - this.BorderThickness.Bottom - this.Padding.Top - this.Padding.Bottom);

            double radius = Math.Max(0.0, this.CornerRadius.TopLeft - this.BorderThickness.Left);

            _clip.Rect = new Rect(new Point(0, 0), new Size(width, height));
            _clip.RadiusX = radius;
            _clip.RadiusY = radius;
        }
    }
}