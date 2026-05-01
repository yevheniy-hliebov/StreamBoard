using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Servers.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamBoard.Features.Servers.Controllers
{
    public class GridDeckController : IHttpController
    {
        private readonly GridDeckStorage _storage;

        public GridDeckController(GridDeckStorage storage)
        {
            _storage = storage;
        }

        public string RoutePrefix => "/api/grid";

        public async Task HandleAsync(HttpListenerContext ctx)
        {
            try
            {
                string path = ctx.Request.Url!.AbsolutePath;
                string method = ctx.Request.HttpMethod;

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

        private async Task GetButtons(HttpListenerContext ctx)
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
            byte[] data = Encoding.UTF8.GetBytes(jsonResponse);

            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength64 = data.Length;
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await ctx.Response.OutputStream.WriteAsync(data);
        }

        private async Task GetImage(HttpListenerContext ctx, string key)
        {
            var profile = _storage.Current;
            var map = profile.CurrentPageButtonMap;

            if (map == null || !map.TryGetValue(key, out var buttonConfig))
            {
                await WriteErrorAsync(ctx, HttpStatusCode.BadRequest, "Key binding data not found");
                return;
            }

            string? imagePath = buttonConfig.ImagePath;
            if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
            {
                await WriteErrorAsync(ctx, HttpStatusCode.NotFound, "Image not found");
                return;
            }

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

            try
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

                ctx.Response.ContentType = mimeType;
                ctx.Response.ContentLength64 = fileBytes.Length;
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                await ctx.Response.OutputStream.WriteAsync(fileBytes);
            }
            catch (Exception)
            {
                await WriteErrorAsync(ctx, HttpStatusCode.InternalServerError, "Error reading image file");
            }
        }

        private async Task ClickKey(HttpListenerContext ctx, string key)
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
                foreach (var action in buttonConfig.Actions)
                {
                    try
                    {
                        await action.ExecuteAsync();
                    }
                    catch
                    {

                    }
                }
            }

            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
            ctx.Response.ContentType = "text/plain";

            byte[] data = System.Text.Encoding.UTF8.GetBytes("Action executed");
            ctx.Response.ContentLength64 = data.Length;

            await ctx.Response.OutputStream.WriteAsync(data);
        }

        private static async Task WriteErrorAsync(HttpListenerContext ctx, HttpStatusCode statusCode, string message)
        {
            ctx.Response.StatusCode = (int)statusCode;
            ctx.Response.ContentType = "text/plain";

            byte[] data = Encoding.UTF8.GetBytes(message);
            ctx.Response.ContentLength64 = data.Length;

            await ctx.Response.OutputStream.WriteAsync(data);
        }
    }
}