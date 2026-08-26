using StreamTabula.Features.Decks.Models;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models.DTO;

public record GridDeckResponseDto(
    [property: JsonPropertyName("grid_layout")] GridLayout GridLayout,
    [property: JsonPropertyName("current_page_id")] string CurrentPageId,
    [property: JsonPropertyName("current_page_name")] string? CurrentPageName,
    [property: JsonPropertyName("page_map")] Dictionary<string, GridButtonDto>? PageMap
);

public record GridButtonDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("background_color")] string BackgroundColor,
    [property: JsonPropertyName("image_path")] string? ImagePath
);
