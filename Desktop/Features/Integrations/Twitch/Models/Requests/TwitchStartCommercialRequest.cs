using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Requests
{
    public class TwitchStartCommercialRequest
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; } = string.Empty;

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}