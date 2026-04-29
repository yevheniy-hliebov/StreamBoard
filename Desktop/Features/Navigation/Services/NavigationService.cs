using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Views.Pages;
using StreamBoard.Features.Home.Pages;
using StreamBoard.Features.Integrations.Common.Views.Pages;
using StreamBoard.Features.Integrations.Obs.Views.Pages;
using StreamBoard.Features.Integrations.Twitch.Views.Pages;
using StreamBoard.Features.Servers.Pages;
using StreamBoard.Features.Settings.Pages;
using StreamBoard.Features.Integrations.Common.Models;
using StreamBoard.Features.Navigation.Models;

namespace StreamBoard.Features.Navigation.Services
{
    public class NavigationService
    {
        private Wpf.Ui.Controls.NavigationView? _navigationControl;

        public IReadOnlyList<AppRoute> AllPages { get; } = [
            new("Home", typeof(HomePage), FluentIcon: FluentIconType.Home, AddSeparatorAfter: true),

            new("Grid Deck", typeof(GridDeckPage), FluentIcon: FluentIconType.Grid, AddSeparatorAfter: true),

            new("Integrations", typeof(IntegrationsPage), FluentIcon: FluentIconType.Puzzle),
            new("OBS Studio", typeof(ObsSettingsPage), IntegrationIcon: IntegrationIconType.Obs, ParentName: "Integrations"),
            new("Twitch", typeof(TwitchSettingsPage), IntegrationIcon: IntegrationIconType.Twitch, ParentName: "Integrations", AddSeparatorAfter: true),

            new("Local Server", typeof(LocalServerPage), FluentIcon: FluentIconType.Network),

            new("Settings", typeof(SettingsPage), FluentIcon: FluentIconType.Settings, IsFooter: true)
        ];

        public void RegisterNavigationControl(Wpf.Ui.Controls.NavigationView navControl)
        {
            _navigationControl = navControl;
        }

        public Type GetPageTypeByName(string name)
        {
            return AllPages.FirstOrDefault(p => p.Name == name)?.PageType ?? typeof(HomePage);
        }

        public void NavigateTo(string pageName)
        {
            if (_navigationControl == null) return;

            var targetType = GetPageTypeByName(pageName);
            _navigationControl.Navigate(targetType);
        }

        public void NavigateTo(Type targetType)
        {
            if (_navigationControl == null) return;
            _navigationControl.Navigate(targetType);
        }
    }
}