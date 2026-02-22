using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StreamBoard.Features.Decks.Views.Components
{
    public partial class ColorPreview : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(string),
                typeof(ColorPreview),
                new FrameworkPropertyMetadata("#2B2B2B", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SelectedColor
        {
            get => (string)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public ColorPreview() => InitializeComponent();

        private void OnPickColorClick(object sender, RoutedEventArgs e)
        {
            using (var colorDialog = new System.Windows.Forms.ColorDialog())
            {
                colorDialog.FullOpen = true;

                try
                {
                    var current = (Color)ColorConverter.ConvertFromString(SelectedColor);
                    colorDialog.Color = System.Drawing.Color.FromArgb(current.R, current.G, current.B);
                }
                catch { }

                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var c = colorDialog.Color;
                    SelectedColor = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                }
            }
        }
    }
}