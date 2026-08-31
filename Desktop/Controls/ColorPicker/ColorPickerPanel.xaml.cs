using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamTabula.Controls.ColorPicker;

public class PresetColor
{
    public string Hex { get; }
    public SolidColorBrush Brush { get; }

    public PresetColor(string hex)
    {
        Hex = hex;
        Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }
}

public partial class ColorPickerPanel : UserControl
{
    private bool _isUpdatingColor = false;

    public ObservableCollection<PresetColor> PresetColors { get; } = [];

    public ColorPickerPanel()
    {
        InitializeComponent();
        InitializePresets();
    }

    private void InitializePresets()
    {
        var defaultHexes = new[]
        {
            "#FF2B2B2B", "#FFFF3B30", "#FFFF9500", "#FFFFCC00", "#FF34C759",
            "#FF00C7BE", "#FF32ADE6", "#FF007AFF", "#FF5856D6", "#FFAF52DE"
        };

        foreach (var hex in defaultHexes)
        {
            PresetColors.Add(new PresetColor(hex));
        }
    }

    public string SelectedColor
    {
        get => (string)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
        nameof(SelectedColor), typeof(string), typeof(ColorPickerPanel),
        new FrameworkPropertyMetadata("#FF2B2B2B", OnSelectedColorChanged));

    public SolidColorBrush SelectedBrush
    {
        get => (SolidColorBrush)GetValue(SelectedBrushProperty);
        set => SetValue(SelectedBrushProperty, value);
    }
    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
        nameof(SelectedBrush), typeof(SolidColorBrush), typeof(ColorPickerPanel),
        new PropertyMetadata(new SolidColorBrush(Color.FromRgb(43, 43, 43))));

    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }
    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue), typeof(double), typeof(ColorPickerPanel), new PropertyMetadata(0.0, OnHsvChanged));

    public double Saturation
    {
        get => (double)GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }
    public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
        nameof(Saturation), typeof(double), typeof(ColorPickerPanel), new PropertyMetadata(0.0, OnHsvChanged));

    public double Brightness
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }
    public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register(
        nameof(Brightness), typeof(double), typeof(ColorPickerPanel), new PropertyMetadata(17.0, OnHsvChanged));

    public Color PureHueColor
    {
        get => (Color)GetValue(PureHueColorProperty);
        set => SetValue(PureHueColorProperty, value);
    }
    public static readonly DependencyProperty PureHueColorProperty = DependencyProperty.Register(
        nameof(PureHueColor), typeof(Color), typeof(ColorPickerPanel), new PropertyMetadata(Colors.Red));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ColorPickerPanel panel && !panel._isUpdatingColor)
            panel.UpdateHsvFromHex(e.NewValue as string);
    }

    private static void OnHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ColorPickerPanel panel && !panel._isUpdatingColor)
            panel.UpdateHexFromHsv();
    }

    private void UpdateHexFromHsv()
    {
        _isUpdatingColor = true;

        PureHueColor = HsvToRgb(Hue, 1.0, 1.0);
        var color = HsvToRgb(Hue, Saturation / 100.0, Brightness / 100.0);

        SelectedColor = $"#FF{color.R:X2}{color.G:X2}{color.B:X2}";
        SelectedBrush = new SolidColorBrush(color);

        _isUpdatingColor = false;
    }

    private void UpdateHsvFromHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex) || !hex.StartsWith("#")) return;
        try
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            RgbToHsv(color, out double h, out double s, out double v);

            _isUpdatingColor = true;
            Hue = h;
            Saturation = s * 100.0;
            Brightness = v * 100.0;
            PureHueColor = HsvToRgb(Hue, 1.0, 1.0);
            SelectedBrush = new SolidColorBrush(color);
            _isUpdatingColor = false;
        }
        catch (FormatException) { }
    }

    private static Color HsvToRgb(double h, double s, double v)
    {
        double c = v * s;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = v - c;

        double r = 0, g = 0, b = 0;

        if (h >= 0 && h < 60) { r = c; g = x; b = 0; }
        else if (h >= 60 && h < 120) { r = x; g = c; b = 0; }
        else if (h >= 120 && h < 180) { r = 0; g = c; b = x; }
        else if (h >= 180 && h < 240) { r = 0; g = x; b = c; }
        else if (h >= 240 && h < 300) { r = x; g = 0; b = c; }
        else if (h >= 300 && h < 360) { r = c; g = 0; b = x; }

        return Color.FromArgb(255,
            (byte)Math.Round((r + m) * 255),
            (byte)Math.Round((g + m) * 255),
            (byte)Math.Round((b + m) * 255));
    }

    private static void RgbToHsv(Color color, out double h, out double s, out double v)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        v = max;
        s = max == 0 ? 0 : delta / max;

        if (delta == 0) h = 0;
        else
        {
            if (max == r) h = 60 * (((g - b) / delta) % 6);
            else if (max == g) h = 60 * (((b - r) / delta) + 2);
            else h = 60 * (((r - g) / delta) + 4);
            if (h < 0) h += 360;
        }
    }

    private void OnPaletteColorClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string hex)
        {
            SelectedColor = hex;
        }
    }
}