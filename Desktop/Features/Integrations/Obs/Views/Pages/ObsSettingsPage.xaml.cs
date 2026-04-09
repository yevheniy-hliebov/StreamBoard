using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Integrations.Obs.ViewModels;

namespace StreamBoard.Features.Integrations.Obs.Views.Pages
{
    public partial class ObsSettingsPage : Page
    {
        public ObsSettingsPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<ObsSettingsViewModel>();
        }
    }
}
