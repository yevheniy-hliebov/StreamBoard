using System.Text.Json.Serialization;

namespace StreamBoard.Features.Servers.Models
{
    public class ServerConfigs
    {
        [JsonPropertyName("http")]
        public HttpServerConfig Http { get; set; } = new();

        // There will be other types of servers
    }
}
