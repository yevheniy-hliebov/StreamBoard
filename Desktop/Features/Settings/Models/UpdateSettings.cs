using System.Text.Json.Serialization;

namespace StreamTabula.Features.Settings.Models;

public class UpdateSettings
{
    [JsonPropertyName("update_channel")]
    public string UpdateChannel { get; set; } = "Stable releases";

    [JsonPropertyName("skipped_version")]
    public string? SkippedVersion { get; set; } = null;
}
