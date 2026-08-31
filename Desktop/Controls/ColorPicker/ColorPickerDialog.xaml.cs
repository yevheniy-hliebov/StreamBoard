using System.Windows;
using StreamTabula.Controls.Dialogs;

namespace StreamTabula.Controls.ColorPicker;

public partial class ColorPickerDialog : BaseDialog
{
    public string ResultColor { get; private set; } = "#FF2B2B2B";

    public string DialogSelectedColor
    {
        get => (string)GetValue(DialogSelectedColorProperty);
        set => SetValue(DialogSelectedColorProperty, value);
    }
    public static readonly DependencyProperty DialogSelectedColorProperty = DependencyProperty.Register(
        nameof(DialogSelectedColor), typeof(string), typeof(ColorPickerDialog), new PropertyMetadata("#FF2B2B2B"));

    public ColorPickerDialog(string initialColor)
    {
        InitializeComponent();
        DialogSelectedColor = string.IsNullOrWhiteSpace(initialColor) ? "#FF2B2B2B" : initialColor;
    }

    private new void Submit(object sender, RoutedEventArgs e)
    {
        ResultColor = DialogSelectedColor;
        DialogResult = true;
        Close();
    }
}