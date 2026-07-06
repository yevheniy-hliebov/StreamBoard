using Microsoft.AspNetCore.Http;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using StreamTabula.Features.Servers.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StreamTabula.Features.Servers.Controllers
{
    public class GridDeckController : IHttpController
    {
        private readonly GridDeckStorage _storage;

        public GridDeckController(GridDeckStorage storage)
        {
            _storage = storage;
        }

        public string RoutePrefix => "/api/grid";

        public async Task HandleAsync(HttpContext ctx)
        {
            try
            {
                string path = ctx.Request.Path.Value ?? "/";
                string method = ctx.Request.Method;

                if (method == "GET" && HttpRouteHelper.TryMatch(path, "/api/grid/buttons", out _))
                {
                    await GetButtons(ctx);
                    return;
                }

                if (method == "GET" && HttpRouteHelper.TryMatch(path, "/api/grid/{key}/image", out var imgParams))
                {
                    await GetImage(ctx, imgParams["key"]);
                    return;
                }

                if (method == "POST" && HttpRouteHelper.TryMatch(path, "/api/grid/{key}", out var postParams))
                {
                    await ClickKey(ctx, postParams["key"]);
                    return;
                }

                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        private async Task GetButtons(HttpContext ctx)
        {
            var profile = _storage.Current;
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

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string jsonResponse = JsonSerializer.Serialize(responseObj, jsonOptions);

            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await ctx.Response.WriteAsync(jsonResponse);
        }

        private async Task GetImage(HttpContext ctx, string key)
        {
            var profile = _storage.Current;
            var map = profile.CurrentPageButtonMap;

            if (map == null || !map.TryGetValue(key, out var buttonConfig))
            {
                await WriteErrorAsync(ctx, HttpStatusCode.BadRequest, "Key binding data not found");
                return;
            }

            string? physicalPath = ResolvePhysicalPath(buttonConfig.ImagePath);

            if (string.IsNullOrEmpty(physicalPath) || !File.Exists(physicalPath))
            {
                await WriteErrorAsync(ctx, HttpStatusCode.NotFound, "Image not found");
                return;
            }

            string extension = Path.GetExtension(physicalPath).ToLowerInvariant();
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

            try
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(physicalPath);

                ctx.Response.ContentType = mimeType;
                ctx.Response.ContentLength = fileBytes.Length;
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                await ctx.Response.Body.WriteAsync(fileBytes);
            }
            catch (Exception)
            {
                await WriteErrorAsync(ctx, HttpStatusCode.InternalServerError, "Error reading image file");
            }
        }

        private async Task ClickKey(HttpContext ctx, string key)
        {
            var profile = _storage.Current;
            var map = profile.CurrentPageButtonMap;

            if (map == null || !map.TryGetValue(key, out var buttonConfig))
            {
                await WriteErrorAsync(ctx, HttpStatusCode.NotFound, "Key binding data not found");
                return;
            }

            if (buttonConfig.Actions != null)
            {
                var context = new ActionExecutionContext();

                foreach (var action in buttonConfig.Actions)
                {
                    try
                    {
                        await action.ExecuteAsync(context);
                    }
                    catch
                    { }
                }
            }

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.ContentType = "text/plain";
            await ctx.Response.WriteAsync("Action executed");
        }

        private static async Task WriteErrorAsync(HttpContext ctx, HttpStatusCode statusCode, string message)
        {
            ctx.Response.StatusCode = (int)statusCode;
            ctx.Response.ContentType = "text/plain";
            await ctx.Response.WriteAsync(message);
        }

        private string? ResolvePhysicalPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (Path.IsPathRooted(path))
            {
                return path;
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "Assets", "Images", "Buttons", path);
        }
    }
}