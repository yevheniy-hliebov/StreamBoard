using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Models.Requests;
using StreamBoard.Features.Integrations.Twitch.Models.Responses;

namespace StreamBoard.Features.Integrations.Twitch.Services.ApiModules
{
    public class TwitchApiChannelModule(TwitchAuthContext context, HttpClient http)
        : TwitchApiModule(context, http)
    {
        public async Task ModifyChannelInfo(string broadcasterId, TwitchModifyChannelRequest requestData)
        {
            string query = $"broadcaster_id={broadcasterId}";

            try
            {
                await SendRequestInternal(HttpMethod.Patch, "/channels", query, requestData);
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to update channel info: {ex.Message}", ex);
            }
        }

        public async Task UpdateTitle(string broadcasterId, string title)
            => await ModifyChannelInfo(broadcasterId, new TwitchModifyChannelRequest { Title = title });

        public async Task SetCategory(string broadcasterId, string gameId)
            => await ModifyChannelInfo(broadcasterId, new TwitchModifyChannelRequest { GameId = gameId });

        public async Task<List<TwitchCategory>> GetCategories(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return [];

            string queryString = $"query={Uri.EscapeDataString(query)}";

            try
            {
                var response = await SendRequestInternal(HttpMethod.Get, "/search/categories", queryString);

                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchCategory>>();

                return result?.Data ?? [];
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to search categories: {ex.Message}", ex);
            }
        }
    }
}