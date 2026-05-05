using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.Twitch.Models
{
    public class TwitchCategory
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("box_art_url")]
        public string BoxArtUrl { get; set; } = string.Empty;
    }
}