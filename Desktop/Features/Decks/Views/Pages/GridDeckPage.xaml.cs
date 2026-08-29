using StreamTabula.Features.Decks.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Decks.Views.Pages;

public partial class GridDeckPage : Page
{
    public GridDeckPage(GridDeckViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
