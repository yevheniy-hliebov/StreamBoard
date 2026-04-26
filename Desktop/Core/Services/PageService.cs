using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Views.Pages;
using StreamBoard.Features.Home.Pages;
using StreamBoard.Features.Integrations.Obs.Views.Pages;
using StreamBoard.Features.Integrations.Twitch.Views.Pages;
using StreamBoard.Features.Servers.Pages;
using StreamBoard.Features.Settings.Pages;

namespace StreamBoard.Core.Services
{
    public class PageService
    {
        public IReadOnlyList<PageInfo> AllPages { get; } = [
            new("Home", typeof(HomePage)),
            new("Grid Deck", typeof(GridDeckPage)),
            new("OBS Studio", typeof(ObsSettingsPage)),
            new("Twitch", typeof(TwitchSettingsPage)),
            new("HTTP Server", typeof(HttpServerPage)),
            new("Settings", typeof(SettingsPage))
        ];

        public Type GetPageTypeByName(string name)
        {
            return AllPages.FirstOrDefault(p => p.Name == name)?.PageType ?? typeof(HomePage);
        }
    }
}