using System.Net.Http;
using System.Net.Http.Json;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Models.Requests;
using StreamTabula.Features.Integrations.Twitch.Models.Responses;

namespace StreamTabula.Features.Integrations.Twitch.Services.ApiModules
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

        public async Task<TwitchCreateClipResponse?> CreateClip(
            string broadcasterId,
            string? title = null,
            float? duration = null
        )
        {
            var queryParams = new List<string> { $"broadcaster_id={broadcasterId}" };

            if (!string.IsNullOrWhiteSpace(title))
                queryParams.Add($"title={Uri.EscapeDataString(title)}");

            if (duration.HasValue)
                queryParams.Add($"duration={duration.Value.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");

            string query = string.Join("&", queryParams);

            try
            {
                var response = await SendRequestInternal(HttpMethod.Post, "/clips", query);

                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchCreateClipResponse>>();

                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to create clip: {ex.Message}", ex);
            }
        }

        public async Task<TwitchStartCommercialResponse?> StartCommercial(
            string broadcasterId,
            int lengthSeconds
        )
        {
            var requestData = new TwitchStartCommercialRequest
            {
                BroadcasterId = broadcasterId,
                Length = lengthSeconds
            };

            try
            {
                var response = await SendRequestInternal(HttpMethod.Post, "/channels/commercial", null, requestData);

                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchStartCommercialResponse>>();

                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to start commercial: {ex.Message}", ex);
            }
        }
    }
}