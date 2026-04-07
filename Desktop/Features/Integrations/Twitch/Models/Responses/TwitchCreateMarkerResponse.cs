using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Requests
{
    public class TwitchCreateMarkerResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("position_seconds")]
        public int PositionSeconds { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}