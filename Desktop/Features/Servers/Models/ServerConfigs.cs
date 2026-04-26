using System.Text.Json.Serialization;

namespace StreamBoard.Features.Servers.Models
{
    public class ServerConfigs
    {
        [JsonPropertyName("local_server")]
        public LocalServerConfig Local { get; set; } = new();

        // There will be other types of servers
    }
}
