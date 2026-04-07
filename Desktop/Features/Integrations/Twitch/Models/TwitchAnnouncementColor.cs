using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public enum TwitchAnnouncementColor
    {
        [JsonPropertyName("primary")] Primary,
        [JsonPropertyName("blue")] Blue,
        [JsonPropertyName("green")] Green,
        [JsonPropertyName("orange")] Orange,
        [JsonPropertyName("purple")] Purple
    }
}