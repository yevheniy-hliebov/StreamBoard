using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models
{
    public class ServerConfigs
    {
        [JsonPropertyName("local_server")]
        public LocalServerConfig Local { get; set; } = new();

        // There will be other types of servers
    }
}
