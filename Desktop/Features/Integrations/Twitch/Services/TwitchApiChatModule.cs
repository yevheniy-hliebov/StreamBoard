using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Models;

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
    }
}