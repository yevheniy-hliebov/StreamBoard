using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.Views.Components.PropertyEditor
{
    public partial class ImagePicker : UserControl
    {
        public ImagePicker() => InitializeComponent();

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ImagePicker), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ImagePicker), new PropertyMetadata(null));

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
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnImagePathChanged)
            );

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
                    if (Path.IsPathRooted(path))
                    {
                        picker.IsImageMissing = !File.Exists(path);
                    }
                    else
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string fullSystemPath = Path.Combine(baseDir, "Assets", "Images", "Buttons", path);
                        picker.IsImageMissing = !File.Exists(fullSystemPath);
                    }
                }
                else
                {
                    picker.IsImageMissing = false;
                }
            }
        }
    }
}