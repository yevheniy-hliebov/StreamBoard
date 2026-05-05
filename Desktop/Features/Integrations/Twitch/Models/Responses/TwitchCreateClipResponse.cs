using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.Twitch.Models.Responses
{
    public class TwitchCreateClipResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("edit_url")]
        public string EditUrl { get; set; } = string.Empty;
    }
}