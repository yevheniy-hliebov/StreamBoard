using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace StreamTabula.Controls.Layout;

[ContentProperty(nameof(WorkspaceContent))]
public partial class PannableWorkspace : UserControl
{
    private Point _start;
    private bool _isDragging;
    private FrameworkElement? _currentContentElement;

    public PannableWorkspace()
    {
        InitializeComponent();

        Focusable = true;
        FocusVisualStyle = null;

        Loaded += OnLoaded;
        Viewport.PreviewMouseDown += (s, e) => this.Focus();
        Viewport.MouseDown += OnViewportMouseDown;
        Viewport.MouseMove += OnMouseMove;
        Viewport.MouseUp += OnMouseUp;
    }

    public object WorkspaceContent
    {
        get => GetValue(WorkspaceContentProperty);
        set => SetValue(WorkspaceContentProperty, value);
    }

    public static readonly DependencyProperty WorkspaceContentProperty =
        DependencyProperty.Register(
            nameof(WorkspaceContent),
            typeof(object),
            typeof(PannableWorkspace),
            new PropertyMetadata(null, OnWorkspaceContentChanged));

    private static void OnWorkspaceContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PannableWorkspace workspace)
        {
            if (workspace._currentContentElement is not null)
            {
                workspace._currentContentElement.SizeChanged -= workspace.OnContentSizeChanged;
            }

            if (e.NewValue is FrameworkElement newElement)
            {
                workspace._currentContentElement = newElement;
                workspace._currentContentElement.SizeChanged += workspace.OnContentSizeChanged;
            }
            else
            {
                workspace._currentContentElement = null;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => CenterSurface();

    private void OnContentSizeChanged(object sender, SizeChangedEventArgs e) => CenterSurface();

    private void OnViewportMouseDown(object sender, MouseButtonEventArgs e)
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
        if (!_isDragging || _currentContentElement == null) return;

        var position = e.GetPosition(Viewport);
        var delta = position - _start;

        double contentWidth = _currentContentElement.ActualWidth;
        double contentHeight = _currentContentElement.ActualHeight;
        double viewportWidth = Viewport.ActualWidth;
        double viewportHeight = Viewport.ActualHeight;

        if (contentWidth == 0 || contentHeight == 0) return;

        double minX = -contentWidth / 2;
        double maxX = viewportWidth - (contentWidth / 2);

        double minY = -contentHeight / 2;
        double maxY = viewportHeight - (contentHeight / 2);

        PanTransform.X = Math.Clamp(PanTransform.X + delta.X, minX, maxX);
        PanTransform.Y = Math.Clamp(PanTransform.Y + delta.Y, minY, maxY);

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

    public void CenterSurface()
    {
        if (_currentContentElement is null) return;

        Dispatcher.InvokeAsync(() =>
        {
            double viewportWidth = Viewport.ActualWidth;
            double viewportHeight = Viewport.ActualHeight;

            double contentWidth = _currentContentElement.ActualWidth;
            double contentHeight = _currentContentElement.ActualHeight;

            if (contentWidth == 0 || contentHeight == 0) return;

            PanTransform.X = (viewportWidth - contentWidth) / 2;
            PanTransform.Y = (viewportHeight - contentHeight) / 2;
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
}