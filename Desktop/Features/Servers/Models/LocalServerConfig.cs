using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models
{
    public class LocalServerConfig
    {
        [JsonPropertyName("address")]
        public string Address { get; set; } = "localhost";

        [JsonPropertyName("port")]
        public int Port { get; set; } = 13550;

        [JsonPropertyName("auto_start")]
        public bool AutoStart { get; set; } = false;

        [JsonIgnore]
        public string HttpPrefix => $"http://{Address}:{Port}/";
    }
}
