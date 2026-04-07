using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Models.Requests;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchApiChatModule(TwitchAuthContext context, HttpClient http)
        : TwitchApiModule(context, http)
    {
        public async Task<TwitchSendMessageResponse?> SendMessage(
            string broadcasterId,
            string senderId,
            string message,
            string? replyParentMessageId = null
        )
        {
            var requestData = new TwitchSendMessageRequest
            {
                BroadcasterId = broadcasterId,
                SenderId = senderId,
                Message = message,
                ReplyParentMessageId = replyParentMessageId
            };

            try
            {
                var response = await SendRequestInternal(HttpMethod.Post, "/chat/messages", null, requestData);
                var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchSendMessageResponse>>();
                return result?.Data?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to send chat message: {ex.Message}", ex);
            }
        }

        public async Task SendAnnouncement(
            string broadcasterId,
            string moderatorId,
            string message,
            TwitchAnnouncementColor color = TwitchAnnouncementColor.Primary
        )
        {
            var query = $"broadcaster_id={broadcasterId}&moderator_id={moderatorId}";

            var requestData = new TwitchSendAnnouncementRequest
            {
                Message = message,
                Color = color
            };

            try
            {
                await SendRequestInternal(HttpMethod.Post, "/chat/announcements", query, requestData);
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to send announcement: {ex.Message}", ex);
            }
        }

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

        public async Task SendShoutout(string fromBroadcasterId, string toBroadcasterId, string moderatorId)
        {
            var query = $"from_broadcaster_id={fromBroadcasterId}&to_broadcaster_id={toBroadcasterId}&moderator_id={moderatorId}";

            try
            {
                await SendRequestInternal(HttpMethod.Post, "/chat/shoutouts", query);
            }
            catch (Exception ex)
            {
                if (ex is Exceptions.TwitchApiException) throw;
                throw new Exception($"Failed to send shoutout: {ex.Message}", ex);
            }
        }
    }
}