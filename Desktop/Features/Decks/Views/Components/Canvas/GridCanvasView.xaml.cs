using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.Views.Components.Canvas
{
    public partial class GridCanvasView : UserControl
    {
        private Point _start;
        private bool _isDragging;

        public GridCanvasView()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Viewport.MouseDown += OnMouseDown;
            Viewport.MouseMove += OnMouseMove;
            Viewport.MouseUp += OnMouseUp;

            DeckSurface.SizeChanged += (_, __) => CenterSurface();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CenterSurface();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _isDragging = true;
                _start = e.GetPosition(Viewport);
                Viewport.CaptureMouse();
                Viewport.Cursor = Cursors.SizeAll;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var position = e.GetPosition(Viewport);
            var delta = position - _start;

            double contentWidth = DeckSurface.ActualWidth;
            double contentHeight = DeckSurface.ActualHeight;
            double viewportWidth = Viewport.ActualWidth;
            double viewportHeight = Viewport.ActualHeight;

            if (contentWidth == 0 || contentHeight == 0) return;

            double minX = -contentWidth / 2;
            double maxX = viewportWidth - (contentWidth / 2);

            double minY = -contentHeight / 2;
            double maxY = viewportHeight - (contentHeight / 2);

            double newX = PanTransform.X + delta.X;
            double newY = PanTransform.Y + delta.Y;

            PanTransform.X = Math.Clamp(newX, minX, maxX);
            PanTransform.Y = Math.Clamp(newY, minY, maxY);

            _start = position;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _isDragging = false;
                Viewport.ReleaseMouseCapture();
                Viewport.Cursor = Cursors.Arrow;
            }
        }

        private void CenterSurface()
        {
            Dispatcher.InvokeAsync(() =>
            {
                double viewportWidth = Viewport.ActualWidth;
                double viewportHeight = Viewport.ActualHeight;

                double contentWidth = DeckSurface.ActualWidth;
                double contentHeight = DeckSurface.ActualHeight;

                if (contentWidth == 0 || contentHeight == 0)
                    return;

                PanTransform.X = (viewportWidth - contentWidth) / 2;
                PanTransform.Y = (viewportHeight - contentHeight) / 2;
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }
}