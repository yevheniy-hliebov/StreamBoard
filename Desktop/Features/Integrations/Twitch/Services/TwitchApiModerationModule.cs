using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Models;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchApiModerationModule(TwitchAuthContext context, HttpClient http)
        : TwitchApiModule(context, http)
    {
        public async Task DeleteChatMessages(string broadcasterId, string moderatorId, string? messageId = null)
        {
            var query = $"broadcaster_id={broadcasterId}&moderator_id={moderatorId}";

            if (!string.IsNullOrWhiteSpace(messageId))
            {
                query += $"&message_id={messageId}";
            }

            try
            {
                await SendRequestInternal(HttpMethod.Delete, "/moderation/chat", query);
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to delete chat messages: {ex.Message}", ex);
            }
        }

        public async Task ClearChat(string broadcasterId, string moderatorId)
            => await DeleteChatMessages(broadcasterId, moderatorId);

        public async Task DeleteMessage(string broadcasterId, string moderatorId, string messageId)
            => await DeleteChatMessages(broadcasterId, moderatorId, messageId);

        public async Task<TwitchShieldModeStatus?> GetShieldModeStatus(string broadcasterId, string moderatorId)
        {
            var query = $"broadcaster_id={broadcasterId}&moderator_id={moderatorId}";

            try
            {
                var response = await SendRequestInternal(HttpMethod.Get, "/moderation/shield_mode", query);
                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchShieldModeStatus>>();
                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to get Shield Mode status: {ex.Message}", ex);
            }
        }

        public async Task<TwitchShieldModeStatus?> UpdateShieldModeStatus(string broadcasterId, string moderatorId, bool isActive)
        {
            var query = $"broadcaster_id={broadcasterId}&moderator_id={moderatorId}";
            var requestData = new TwitchUpdateShieldModeRequest { IsActive = isActive };

            try
            {
                var response = await SendRequestInternal(HttpMethod.Put, "/moderation/shield_mode", query, requestData);
                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchShieldModeStatus>>();
                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to update Shield Mode status: {ex.Message}", ex);
            }
        }

        public async Task ToggleShieldMode(string broadcasterId, string moderatorId, bool isActive)
            => await UpdateShieldModeStatus(broadcasterId, moderatorId, isActive);
    }
}