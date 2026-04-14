using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Integrations.Twitch.ViewModels;

namespace StreamBoard.Features.Integrations.Twitch.Views.Pages
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