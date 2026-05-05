using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Decks.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Decks.Views.Pages
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
