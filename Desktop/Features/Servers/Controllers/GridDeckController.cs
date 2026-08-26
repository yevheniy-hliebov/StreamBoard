using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using StreamTabula.Features.Servers.Models.DTO;
using System.Diagnostics;

namespace StreamTabula.Features.Servers.Controllers;

[ApiController]
[Route("api/grid")]
public class GridDeckController(GridDeckStorage storage) : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    [HttpGet("buttons")]
    public IActionResult GetButtons()
    {
        var profile = storage.Current;

        if (profile.CanvasConfig is not GridCanvasConfig gridCanvasConfig)
        {
            return BadRequest("Invalid canvas configuration type.");
        }

        var selectedPage = profile.PagesState.AllPages.FirstOrDefault(p => p.Id == profile.PagesState.SelectedPageId);

        var pageMap = profile.CurrentPageButtonMap?.ToDictionary(
            kvp => kvp.Key,
            kvp => new GridButtonDto(
                kvp.Value.Name,
                kvp.Value.BackgroundColor,
                kvp.Value.ImagePath
            )
        );

        var response = new GridDeckResponseDto(
            gridCanvasConfig.SelectedGrid,
            profile.PagesState.SelectedPageId,
            selectedPage?.Name,
            pageMap
        );

        return Ok(response);
    }

    [HttpGet("{key}/image")]
    public IActionResult GetImage(string key)
    {
        if (!TryGetButtonConfig(key, out var buttonConfig))
            return NotFound("Key binding data not found");

        string? imagePath = buttonConfig.ImagePath;
        if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
            return NotFound("Image not found");

        if (!_contentTypeProvider.TryGetContentType(imagePath, out var mimeType))
        {
            mimeType = "application/octet-stream";
        }

        return PhysicalFile(imagePath, mimeType);
    }

    [HttpPost("{key}")]
    public async Task<IActionResult> ClickKey(string key)
    {
        if (!TryGetButtonConfig(key, out var buttonConfig))
            return NotFound("Key binding data not found");

        if (buttonConfig.Actions != null)
        {
            foreach (var action in buttonConfig.Actions)
            {
                try
                {
                    await action.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to execute action for key {key}: {ex.Message}");
                }
            }
        }

        return Ok("Key pressed");
    }

    private bool TryGetButtonConfig(string key, out DeckButtonConfig config)
    {
        var map = storage.Current.CurrentPageButtonMap;
        if (map != null && map.TryGetValue(key, out config!))
        {
            return true;
        }

        config = null!;
        return false;
    }
}