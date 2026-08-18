using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Integrations.Obs.ViewModels;

namespace StreamTabula.Features.Integrations.Obs.Views.Pages
{
    public partial class ObsSettingsPage : Page
    {
        public ObsSettingsPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<OBSSettingsViewModel>();
        }
    }
}
