using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.ViewModels;
using System.Windows.Controls;

namespace StreamBoard.Features.Servers.Pages
{
    public partial class HttpServerPage : Page
    {
        public HttpServerPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<HttpServerViewModel>();
        }
    }
}
