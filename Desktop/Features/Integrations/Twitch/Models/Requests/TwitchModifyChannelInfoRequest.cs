using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Requests
{
    public class TwitchModifyChannelRequest
    {
        [JsonPropertyName("game_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GameId { get; set; }

        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [JsonPropertyName("broadcaster_language")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BroadcasterLanguage { get; set; }

        [JsonPropertyName("tags")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Tags { get; set; }

        [JsonPropertyName("is_branded_content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsBrandedContent { get; set; }
    }
}