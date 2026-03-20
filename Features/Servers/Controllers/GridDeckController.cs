using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Controllers
{
    public class GridDeckController : IHttpController
    {
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
            string responseText = "Grid Deck Buttons";
            byte[] data = Encoding.UTF8.GetBytes(responseText);

            ctx.Response.ContentType = "text/plain";
            ctx.Response.ContentLength64 = data.Length;
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            await ctx.Response.OutputStream.WriteAsync(data);
            await Task.CompletedTask;
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