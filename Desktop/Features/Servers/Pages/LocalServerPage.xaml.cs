using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Servers.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Servers.Pages
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
