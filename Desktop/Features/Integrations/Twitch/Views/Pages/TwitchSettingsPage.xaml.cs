using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Integrations.Twitch.ViewModels;

namespace StreamTabula.Features.Integrations.Twitch.Views.Pages
{
    public partial class TwitchSettingsPage : Page
    {
        public TwitchSettingsPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<TwitchSettingsViewModel>();
        }
    }
}