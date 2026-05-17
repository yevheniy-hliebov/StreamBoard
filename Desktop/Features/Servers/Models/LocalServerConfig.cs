using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models
{
    public class LocalServerConfig
    {
        [JsonPropertyName("port")]
        public int Port { get; set; } = 13550;

        [JsonPropertyName("auto_start")]
        public bool AutoStart { get; set; } = false;
    }
}
