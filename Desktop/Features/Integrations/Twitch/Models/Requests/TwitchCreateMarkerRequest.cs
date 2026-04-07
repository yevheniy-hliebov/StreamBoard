using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Requests
{
    public class TwitchCreateMarkerRequest
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
    }
}