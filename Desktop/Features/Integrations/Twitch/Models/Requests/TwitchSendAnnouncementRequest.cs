using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.Twitch.Models.Requests
{
    public class TwitchSendAnnouncementRequest
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = TwitchAnnouncementColor.primary.ToString().ToLower();
    }
}