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
            var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
            return gateway?.Broadcaster.User?.Login ?? string.Empty;
        }
    }

    public class AnnouncementColorsOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(BaseAction action)
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

    public class TwitchChatModeOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(BaseAction action)
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
        public List<string> GetOptions(BaseAction action)
        {
            return ["Toggle", "Enable", "Disable"];
        }
    }
}