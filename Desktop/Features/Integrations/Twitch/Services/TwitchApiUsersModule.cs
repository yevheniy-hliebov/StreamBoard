using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Exceptions;
using StreamBoard.Features.Integrations.Twitch.Models;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchApiUsersModule(TwitchAuthContext context, HttpClient http)
        : TwitchApiModule(context, http)
    {
        public async Task<TwitchUserIdentify?> GetMe() => await GetUser();

        public async Task<TwitchUserIdentify?> GetUser(string? login = null)
        {
            string? query = null;
            if (login != null && login.Trim() != "")
            {
                query = $"login={login}";
            }

            try
            {
                var response = await SendRequestInternal(HttpMethod.Get, "/users", query);
                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchUserIdentify>>();
                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is TwitchApiException) throw;
                throw new Exception($"Failed to get Twitch user: {ex.Message}", ex);
            }
        }
    }
}