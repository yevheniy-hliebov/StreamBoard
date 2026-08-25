using Microsoft.AspNetCore.Mvc;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using System.Diagnostics;

namespace StreamTabula.Features.Servers.Controllers;

[ApiController]
[Route("api/grid")]
public class GridDeckController(GridDeckStorage storage) : ControllerBase
{
    [HttpGet("buttons")]
    public IActionResult GetButtons()
    {
        var profile = storage.Current;
        var map = profile.CurrentPageButtonMap;
        var selectedPage = profile.PagesState.AllPages.FirstOrDefault(p => p.Id == profile.PagesState.SelectedPageId);
        var gridCanvasConfig = (GridCanvasConfig)profile.CanvasConfig;

        var responseObj = new
        {
            grid_layout = gridCanvasConfig.SelectedGrid,
            current_page_id = profile.PagesState.SelectedPageId,
            current_page_name = selectedPage?.Name,
            page_map = map?.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    name = kvp.Value.Name,
                    background_color = kvp.Value.BackgroundColor,
                    image_path = kvp.Value.ImagePath
                }
            )
        };

        return Ok(responseObj);
    }

    [HttpGet("{key}/image")]
    public IActionResult GetImage(string key)
    {
        var map = storage.Current.CurrentPageButtonMap;

        if (map == null || !map.TryGetValue(key, out var buttonConfig))
            return BadRequest("Key binding data not found");

        string? imagePath = buttonConfig.ImagePath;
        if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
            return NotFound("Image not found");

        string extension = System.IO.Path.GetExtension(imagePath).ToLowerInvariant();
        string mimeType = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };

        return PhysicalFile(imagePath, mimeType);
    }

    [HttpPost("{key}")]
    public async Task<IActionResult> ClickKey(string key)
    {
        var map = storage.Current.CurrentPageButtonMap;

        if (map == null || !map.TryGetValue(key, out var buttonConfig))
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

        return Ok("Action executed");
    }
}