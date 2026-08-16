using System.Text.Json.Serialization;
using StreamTabula.Core.Models;
using StreamTabula.Features.Integrations.Obs.Models;

namespace StreamTabula.Features.Integrations.Common.Models;

public class IntegrationConnectionSettings : IVersionedConfig
{
    [JsonPropertyName("config_version")]
    public int ConfigVersion { get; set; } = 1;

    [JsonPropertyName("obs")]
    public ObsConnectionSettings Obs { get; set; } = new();
}