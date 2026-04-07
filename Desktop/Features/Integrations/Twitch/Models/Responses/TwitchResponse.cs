using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models.Responses
{
    public class TwitchResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = [];
    }
}