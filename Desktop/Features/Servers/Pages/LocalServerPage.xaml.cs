using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.ViewModels;
using System.Windows.Controls;

namespace StreamBoard.Features.Servers.Pages
{
    public partial class LocalServerPage : Page
    {
        public LocalServerPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<LocalServerViewModel>();
        }
    }
}
