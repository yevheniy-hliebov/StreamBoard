using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Controls.Inputs;

public partial class ImagePicker : UserControl
{
    public ImagePicker() => InitializeComponent();

    public string ImagePath
    {
        get { return (string)GetValue(ImagePathProperty); }
        set { SetValue(ImagePathProperty, value); }
    }

    public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.Register(
            nameof(ImagePath),
            typeof(string),
            typeof(ImagePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnImagePathChanged));

    public bool IsImageMissing
    {
        get { return (bool)GetValue(IsImageMissingProperty); }
        private set { SetValue(IsImageMissingPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey IsImageMissingPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(IsImageMissing),
            typeof(bool),
            typeof(ImagePicker),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsImageMissingProperty = IsImageMissingPropertyKey.DependencyProperty;

    private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImagePicker picker)
        {
            string? path = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(path))
            {
                picker.IsImageMissing = !File.Exists(path);
            }
            else
            {
                picker.IsImageMissing = false;
            }
        }
    }

    private void OnPickerClicked(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select an Image",
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            ImagePath = openFileDialog.FileName;
        }
    }
}