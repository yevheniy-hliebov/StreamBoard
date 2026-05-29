using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Settings.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Settings.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
        }
    }
}
