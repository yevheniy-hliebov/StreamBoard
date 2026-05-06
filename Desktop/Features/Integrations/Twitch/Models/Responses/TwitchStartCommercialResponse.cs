using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.Twitch.Models.Responses
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