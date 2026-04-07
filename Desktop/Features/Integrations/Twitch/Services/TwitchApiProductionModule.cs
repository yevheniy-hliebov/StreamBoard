using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Models.Requests;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchApiProductionModule(TwitchAuthContext context, HttpClient http)
        : TwitchApiModule(context, http)
    {
        public async Task<TwitchCreateMarkerResponse?> CreateStreamMarker(
            string userId,
            string? description = null
        )
        {
            var requestData = new TwitchCreateMarkerRequest
            {
                UserId = userId,
                Description = description
            };

            try
            {
                var response = await SendRequestInternal(HttpMethod.Post, "/streams/markers", null, requestData);

                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchCreateMarkerResponse>>();

                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to create stream marker: {ex.Message}", ex);
            }
        }
    }
}