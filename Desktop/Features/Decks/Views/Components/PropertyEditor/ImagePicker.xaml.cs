using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.Views.Components.PropertyEditor
{
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
                // Додаємо OnImagePathChanged для реакції на зміну шляху
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnImagePathChanged)
            );

        // Нова властивість: чи втрачено/не знайдено файл
        public bool IsImageMissing
        {
            get { return (bool)GetValue(IsImageMissingProperty); }
            private set { SetValue(IsImageMissingPropertyKey, value); }
        }

        // Робимо її ReadOnly ззовні, щоб ніхто не міг зламати логіку
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

        private void OnPickerClicked(object sender, MouseButtonEventArgs e)
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
}