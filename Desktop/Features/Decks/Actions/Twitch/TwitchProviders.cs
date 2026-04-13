using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    public class TwitchUsernameProvider : IValueProvider
    {
        public string GetValue(DeckAction action)
        {
            var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
            return gateway?.Broadcaster.User?.Login ?? string.Empty;
        }
    }

    public class AnnouncementColorsOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(DeckAction action)
        {
            return Enum.GetNames<TwitchAnnouncementColor>().ToList();
        }
    }

    public class TwitchCategorySearchProvider : IAsyncSearchProvider
    {
        public async Task<IEnumerable<SearchResult>> SearchAsync(string query)
        {
            var gateway = App.ServiceProvider.GetService<TwitchAccountsGateway>();
            if (gateway?.Broadcaster.Api == null) return [];

            try
            {
                var categories = await gateway.Broadcaster.Api.Channel.GetCategories(query);
                return categories.Select(c => new SearchResult { Id = c.Id, DisplayName = c.Name });
            }
            catch
            {
                return [];
            }
        }
    }
}