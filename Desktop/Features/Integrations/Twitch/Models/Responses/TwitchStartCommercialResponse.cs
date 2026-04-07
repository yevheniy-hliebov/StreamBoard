using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchStartCommercialResponse
    {
        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("retry_after")]
        public int RetryAfter { get; set; }
    }
}