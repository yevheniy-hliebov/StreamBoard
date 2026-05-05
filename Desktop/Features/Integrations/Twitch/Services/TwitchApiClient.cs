using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

namespace StreamTabula.Features.Integrations.Twitch.Services
{
    public class TwitchApiClient(TwitchAuthContext context, HttpClient http, IMemoryCache cache)
    {
        public TwitchApiUsersModule Users { get; } = new(context, http, cache);
        public TwitchApiChannelModule Channel { get; } = new(context, http);
        public TwitchApiChatModule Chat { get; } = new(context, http);
        public TwitchApiChatSettingsModule ChatSettings { get; } = new(context, http);
        public TwitchApiModerationModule Moderation { get; } = new(context, http);
        public TwitchApiProductionModule Production { get; } = new(context, http);
    }
}