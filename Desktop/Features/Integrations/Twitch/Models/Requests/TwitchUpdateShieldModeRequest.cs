using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchUpdateShieldModeRequest
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }
}