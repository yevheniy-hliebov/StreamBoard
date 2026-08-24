using StreamTabula.Features.Servers.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Servers.Views.Pages;

public partial class LocalServerPage : Page
{
    public LocalServerPage(LocalServerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
