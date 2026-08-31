using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Controls.ColorPicker;

public partial class ColorPickerButton : UserControl
{
    public ColorPickerButton() => InitializeComponent();

    public string SelectedColor
    {
        get => (string)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
        nameof(SelectedColor),
        typeof(string),
        typeof(ColorPickerButton),
        new FrameworkPropertyMetadata("#FF2B2B2B", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private void OnPickColorClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ColorPickerDialog(SelectedColor)
        {
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedColor = dialog.ResultColor;
        }
    }
}