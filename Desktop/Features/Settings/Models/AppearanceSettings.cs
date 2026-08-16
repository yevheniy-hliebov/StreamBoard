using System.Text.Json.Serialization;

namespace StreamTabula.Features.Settings.Models;

public class AppearanceSettings
{
    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "Dark";
}
