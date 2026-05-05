using System.Text.Json.Serialization;
using StreamTabula.Features.Integrations.Obs.Models;

namespace StreamTabula.Features.Integrations.Common.Models
{
    public class IntegrationConnectionSettings
    {
        [JsonPropertyName("obs")]
        public ObsConnectionSettings Obs { get; set; } = new();
    }
}