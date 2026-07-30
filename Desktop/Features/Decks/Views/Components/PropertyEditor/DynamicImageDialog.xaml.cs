using StreamTabula.Components.Dialogs;
using StreamTabula.Features.Decks.ViewModels;
using System.Text.Json;
using System.Windows;
using StreamTabula.Features.Decks.Models;

namespace StreamTabula.Features.Decks.Views.Components.PropertyEditor;

public partial class DynamicImageDialog : BaseDialog
{
    public DynamicImageModel? Result { get; private set; }

    public DynamicImageDialog(DynamicImageModel originalModel)
    {
        InitializeComponent();

        string json = JsonSerializer.Serialize(originalModel);
        var copy = JsonSerializer.Deserialize<DynamicImageModel>(json) ?? new DynamicImageModel();

        DataContext = new DynamicImageViewModel(copy);
    }

    protected override void Submit(object sender, RoutedEventArgs e)
    {
        if (DataContext is DynamicImageViewModel viewModel)
        {
            Result = viewModel.DynamicImage;
        }

        DialogResult = true;
        Close();
    }
}