using System.Text.Json.Serialization;
using StreamBoard.Features.Integrations.Obs.Models;

namespace StreamBoard.Features.Integrations.Common.Models
{
    public class IntegrationConnectionSettings
    {
        [JsonPropertyName("obs")]
        public ObsConnectionSettings Obs { get; set; } = new();
    }
}