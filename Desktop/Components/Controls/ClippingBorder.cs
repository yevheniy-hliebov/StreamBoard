using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamBoard.Components.Controls
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

            double radius = Math.Max(0.0, CornerRadius.TopLeft - (BorderThickness.Left * 0.5));

            _clip.Rect = new Rect(new Point(0, 0), this.RenderSize);
            _clip.RadiusX = radius;
            _clip.RadiusY = radius;
        }
    }
}