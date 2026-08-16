using System.Text.Json.Serialization;

namespace StreamTabula.Features.Settings.Models;

public class WindowSettings
{
    [JsonPropertyName("width")]
    public double Width { get; set; } = 1000;

    [JsonPropertyName("height")]
    public double Height { get; set; } = 600;

    [JsonPropertyName("left")]
    public double Left { get; set; } = 0;

    [JsonPropertyName("top")]
    public double Top { get; set; } = 0;

    [JsonPropertyName("is_maximized")]
    public bool IsMaximized { get; set; } = false;
}