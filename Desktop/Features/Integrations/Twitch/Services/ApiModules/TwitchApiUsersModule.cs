using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Features.Integrations.Twitch.Exceptions;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Models.Responses;

namespace StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

public class TwitchApiUsersModule(ITwitchSession session, HttpClient http, IMemoryCache cache)
    : TwitchApiModule(session, http)
{
    private readonly IMemoryCache _cache = cache;

    public async Task<TwitchUserIdentity?> GetMe(TwitchAuthContext? overrideContext = null)
        => await GetUser(login: null, overrideContext: overrideContext);

    public async Task<TwitchUserIdentity?> GetUser(string? login = null, TwitchAuthContext? overrideContext = null)
    {
        string? query = null;
        bool isSpecificLogin = !string.IsNullOrWhiteSpace(login);

        if (isSpecificLogin)
        {
            query = $"login={login!.Trim()}";
        }

        try
        {
            var response = await SendRequestInternal(
                HttpMethod.Get,
                "/users",
                query,
                overrideContext: overrideContext);

            var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchUserIdentity>>();
            var user = result?.Data?.FirstOrDefault();

            if (user != null)
            {
                CacheUserId(user.Login, user.Id);

                if (isSpecificLogin)
                {
                    CacheUserId(login!, user.Id);
                }
            }

            return user;
        }
        catch (Exception ex)
        {
            if (ex is TwitchApiException) throw;
            throw new InvalidOperationException($"Failed to get Twitch user: {ex.Message}", ex);
        }
    }

    public async Task<string?> GetUserIdByLogin(string login)
    {
        string cleanLogin = login.ToLower().Trim();
        string cacheKey = $"user_id_{cleanLogin}";

        if (_cache.TryGetValue(cacheKey, out string? cachedId))
        {
            return cachedId;
        }

        var user = await GetUser(cleanLogin);
        return user?.Id;
    }

    private void CacheUserId(string login, string id)
    {
        string cleanLogin = login.ToLower().Trim();
        string cacheKey = $"user_id_{cleanLogin}";

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(1));

        _cache.Set(cacheKey, id, cacheOptions);
    }
}