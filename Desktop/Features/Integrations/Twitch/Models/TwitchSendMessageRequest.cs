using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchSendMessageRequest
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; } = string.Empty;

        [JsonPropertyName("sender_id")]
        public string SenderId { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("reply_parent_message_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReplyParentMessageId { get; set; }
    }
}