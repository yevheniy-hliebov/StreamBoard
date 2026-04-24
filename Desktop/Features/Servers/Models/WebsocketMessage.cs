using System.Text.Json.Serialization;

namespace StreamBoard.Features.Servers.Models
{
    public class WebsocketMessage(WebsocketMessageType type, object data)
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = type.ToString();

        [JsonPropertyName("data")]
        public object Data { get; set; } = data;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}