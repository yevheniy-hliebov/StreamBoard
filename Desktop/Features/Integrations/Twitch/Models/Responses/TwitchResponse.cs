using System.Text.Json.Serialization;

namespace StreamTabula.Features.Integrations.Twitch.Models.Responses
{
    public class TwitchResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = [];
    }
}