using StreamTabula.Core.Models;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models;

public class ServerConfigs : IVersionedConfig
{
    [JsonPropertyName("config_version")]
    public int ConfigVersion { get; set; } = 1;

    [JsonPropertyName("local_server")]
    public LocalServerConfig Local { get; set; } = new();
}
