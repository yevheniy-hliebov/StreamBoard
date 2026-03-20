using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Servers.Models;
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

        public string RoutePrefix => "/grid";

        public async Task HandleAsync(HttpListenerContext ctx)
        {
            try
            {
                string path = ctx.Request.Url!.AbsolutePath;
                string method = ctx.Request.HttpMethod;

                if (method == "GET" && HttpRouteHelper.TryMatch(path, "/grid/buttons", out _))
                {
                    await GetButtons(ctx);
                    return;
                }

                if (method == "GET" && HttpRouteHelper.TryMatch(path, "/grid/{key}/image", out var imgParams))
                {
                    await GetImage(ctx, imgParams["key"]);
                    return;
                }

                if (method == "POST" && HttpRouteHelper.TryMatch(path, "/grid/{key}", out var postParams))
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
            var profile = _storage.CurrentProfile;
            var map = profile.CurrentPageButtonMap;

            var responseObj = new
            {
                grid_template = profile.CanvasConfig.SelectedGrid,
                current_page_id = profile.Pages.SelectedPageId,
                
                // Перебираємо словник кнопок і залишаємо лише потрібні поля для відображення
                page_map = map?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        key_name = kvp.Value.Name,
                        key_background_color = kvp.Value.BackgroundColor,
                        key_image_path = kvp.Value.ImagePath
                    }
                )
            };

            // Серіалізуємо в JSON
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
            string responseText = $"Button {key} Image";
            byte[] data = Encoding.UTF8.GetBytes(responseText);

            ctx.Response.ContentType = "text/plain";
            ctx.Response.ContentLength64 = data.Length;
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await ctx.Response.OutputStream.WriteAsync(data);
            await Task.CompletedTask;
        }

        private async Task ClickKey(HttpListenerContext ctx, string key)
        {
            string responseText = $"Clicked button {key}";
            byte[] data = Encoding.UTF8.GetBytes(responseText);

            ctx.Response.ContentType = "text/plain";
            ctx.Response.ContentLength64 = data.Length;
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await ctx.Response.OutputStream.WriteAsync(data);
            await Task.CompletedTask;
        }
    }
}