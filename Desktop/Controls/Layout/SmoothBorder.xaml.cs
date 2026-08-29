using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamTabula.Controls.Layout;

public partial class SmoothBorder : UserControl
{
    public SmoothBorder() => InitializeComponent();

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SmoothBorder),
            new PropertyMetadata(new CornerRadius(0), OnLayoutChanged));

    public CornerRadius InnerCornerRadius
    {
        get => (CornerRadius)GetValue(InnerCornerRadiusProperty);
        private set => SetValue(InnerCornerRadiusProperty, value);
    }
    public static readonly DependencyProperty InnerCornerRadiusProperty =
        DependencyProperty.Register(nameof(InnerCornerRadius), typeof(CornerRadius), typeof(SmoothBorder),
            new PropertyMetadata(new CornerRadius(0)));

    public new Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    public static readonly new DependencyProperty BorderThicknessProperty =
        DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(SmoothBorder),
            new PropertyMetadata(new Thickness(0), OnLayoutChanged));

    public new Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }
    public static readonly new DependencyProperty BorderBrushProperty =
        DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(SmoothBorder), new PropertyMetadata(null));

    public new Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
    public static readonly new DependencyProperty BackgroundProperty =
        DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(SmoothBorder), new PropertyMetadata(null));

    public Brush OverlayBrush
    {
        get => (Brush)GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }
    public static readonly DependencyProperty OverlayBrushProperty =
        DependencyProperty.Register(nameof(OverlayBrush), typeof(Brush), typeof(SmoothBorder), new PropertyMetadata(null));

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }
    public static readonly DependencyProperty InnerContentProperty =
        DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(SmoothBorder), new PropertyMetadata(null));

    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SmoothBorder control)
        {
            double tL = Math.Max(0, control.CornerRadius.TopLeft - control.BorderThickness.Left);
            double tR = Math.Max(0, control.CornerRadius.TopRight - control.BorderThickness.Right);
            double bR = Math.Max(0, control.CornerRadius.BottomRight - control.BorderThickness.Right);
            double bL = Math.Max(0, control.CornerRadius.BottomLeft - control.BorderThickness.Left);

            control.InnerCornerRadius = new CornerRadius(tL, tR, bR, bL);
        }
    }
}