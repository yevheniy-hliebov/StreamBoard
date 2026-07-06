using Microsoft.Win32;
using StreamTabula.Components.Dialogs;
using StreamTabula.Features.Decks.Models;
using System.IO;
using System.Windows;

namespace StreamTabula.Features.Decks.Views.Components.PropertyEditor;

public partial class ImageLibraryDialog : BaseDialog
{
    public ImageLibraryResult? Result { get; private set; }

    public List<ImageItem> SystemImages { get; set; } = [];

    private ImageItem? _selectedImage;
    public ImageItem? SelectedImage
    {
        get => _selectedImage;
        set
        {
            _selectedImage = value;
            if (_selectedImage != null)
            {
                Result = new ImageLibraryResult(ImageType.System, _selectedImage.FileName);
                DialogResult = true;
                Close();
            }
        }
    }

    public ImageLibraryDialog()
    {
        LoadSystemImages();
        InitializeComponent();
    }

    private void LoadSystemImages()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string imagesPath = Path.Combine(baseDir, "Assets", "Images", "Buttons");

        if (!Directory.Exists(imagesPath))
            return;

        var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".svg", ".webp" };

        SystemImages = Directory.GetFiles(imagesPath)
            .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
            .Select(file => new ImageItem
            {
                FileName = Path.GetFileName(file),
                FullPath = file
            })
            .ToList();
    }

    private void OnChooseCustomImageClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select Custom Image",
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.svg;*.webp|All Files|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            Result = new ImageLibraryResult(ImageType.Custom, openFileDialog.FileName);
            DialogResult = true;
            Close();
        }
    }
}

public class ImageItem
{
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
}
