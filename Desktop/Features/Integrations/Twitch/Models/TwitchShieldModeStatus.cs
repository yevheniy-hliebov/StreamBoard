using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchShieldModeStatus
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("moderator_id")]
        public string ModeratorId { get; set; } = string.Empty;

        [JsonPropertyName("moderator_login")]
        public string ModeratorLogin { get; set; } = string.Empty;

        [JsonPropertyName("moderator_name")]
        public string ModeratorName { get; set; } = string.Empty;

        [JsonPropertyName("last_activated_at")]
        public string LastActivatedAt { get; set; } = string.Empty;
    }
}