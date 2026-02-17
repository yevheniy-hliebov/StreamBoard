using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.GridDeck.ViewModels;
using System.Windows.Controls;

namespace StreamBoard.Features.Decks.Views.Pages
{
    public partial class GridDeckPage : Page
    {
        public GridDeckPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<GridDeckViewModel>();
        }
    }
}
