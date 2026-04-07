using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Responses
{
    public class TwitchSendMessageResponse
    {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; } = string.Empty;

        [JsonPropertyName("is_sent")]
        public bool IsSent { get; set; }

        [JsonPropertyName("drop_reason")]
        public TwitchDropReason? DropReason { get; set; }
    }

    public class TwitchDropReason
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}