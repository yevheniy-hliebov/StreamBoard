using StreamTabula.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Settings.Models;

public class SettingsModel : IVersionedConfig
{
    [JsonPropertyName("config_version")]
    public int ConfigVersion { get; set; } = 2;

    [JsonPropertyName("startup")]
    public StartupSettings Startup { get; set; } = new();

    [JsonPropertyName("appearance")]
    public AppearanceSettings Appearance { get; set; } = new();

    [JsonPropertyName("updates")]
    public UpdateSettings Updates { get; set; } = new();

    [JsonPropertyName("window")]
    public WindowSettings Window { get; set; } = new();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? OldProperties { get; set; }
}