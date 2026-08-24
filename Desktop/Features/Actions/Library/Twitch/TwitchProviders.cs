using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    public class TwitchUsernameProvider : IValueProvider
    {
        public string GetValue(BaseAction action)
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            return gateway?.Broadcaster.Session.User?.Login ?? string.Empty;
        }
    }

    public class AnnouncementColorsOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction action)
        {
            return Enum.GetNames<TwitchAnnouncementColor>().ToList();
        }
    }

    public class TwitchCategorySearchProvider : IAsyncSearchProvider
    {
        public async Task<IEnumerable<SearchResult>> SearchAsync(string query)
        {
            var gateway = App.ServiceProvider.GetService<ITwitchAccountsGateway>();
            if (gateway == null) return [];

            try
            {
                var categories = await gateway!.Broadcaster.Api.Channel.GetCategories(query);
                return categories.Select(c => new SearchResult { Id = c.Id, DisplayName = c.Name });
            }
            catch
            {
                return [];
            }
        }
    }

    public class TwitchChatModeOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction action)
        {
            return
            [
                "Emote-Only",
                "Followers-Only",
                "Subscribers-Only",
                "Slow Mode"
            ];
        }
    }

    public class TwitchShieldModeStateOptionsProvider : IOptionsProvider
    {
        public IEnumerable<object> GetOptions(BaseAction action)
        {
            return ["Toggle", "Enable", "Disable"];
        }
    }
}