using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using StreamBoard.Features.Integrations.Twitch.Exceptions;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Models.Responses;

namespace StreamBoard.Features.Integrations.Twitch.Services.ApiModules
{
    public class TwitchApiUsersModule(TwitchAuthContext context, HttpClient http, IMemoryCache cache)
        : TwitchApiModule(context, http)
    {
        private readonly IMemoryCache _cache = cache;

        public async Task<TwitchUserIdentify?> GetMe() => await GetUser();

        public async Task<TwitchUserIdentify?> GetUser(string? login = null)
        {
            string? query = null;
            bool isSpecificLogin = !string.IsNullOrWhiteSpace(login);

            if (isSpecificLogin)
            {
                query = $"login={login!.Trim()}";
            }

            try
            {
                var response = await SendRequestInternal(HttpMethod.Get, "/users", query);
                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchUserIdentify>>();
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
                throw new Exception($"Failed to get Twitch user: {ex.Message}", ex);
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
}