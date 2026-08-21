using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public class TwitchApiClient(ITwitchSession session, HttpClient http, IMemoryCache cache)
{
    public TwitchApiUsersModule Users { get; } = new(session, http, cache);
    public TwitchApiChannelModule Channel { get; } = new(session, http);
    public TwitchApiChatModule Chat { get; } = new(session, http);
    public TwitchApiChatSettingsModule ChatSettings { get; } = new(session, http);
    public TwitchApiModerationModule Moderation { get; } = new(session, http);
    public TwitchApiProductionModule Production { get; } = new(session, http);
}