using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public class TwitchApiClient(ITwitchSession session, HttpClient http, IMemoryCache cache, string clientId)
{
    public TwitchApiUsersModule Users { get; } = new(session, http, cache, clientId);
    public TwitchApiChannelModule Channel { get; } = new(session, http, clientId);
    public TwitchApiChatModule Chat { get; } = new(session, http, clientId);
    public TwitchApiChatSettingsModule ChatSettings { get; } = new(session, http, clientId);
    public TwitchApiModerationModule Moderation { get; } = new(session, http, clientId);
    public TwitchApiProductionModule Production { get; } = new(session, http, clientId);
}