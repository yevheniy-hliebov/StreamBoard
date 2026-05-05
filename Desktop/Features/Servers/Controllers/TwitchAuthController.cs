using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamTabula.Features.Servers.Controllers
{
    public class TwitchAuthController(TwitchAccountsGateway gateway) : IHttpController
    {
        private readonly TwitchAccountsGateway _gateway = gateway;

        public string RoutePrefix => "/twitch";

        public async Task HandleAsync(HttpListenerContext ctx)
        {
            string path = ctx.Request.Url!.AbsolutePath;
            string method = ctx.Request.HttpMethod;

            if (method == "GET" && HttpRouteHelper.TryMatch(path, "/twitch/{type}", out var paramsGet))
            {
                await ServeAuthPage(ctx);
                return;
            }

            if (method == "POST" && HttpRouteHelper.TryMatch(path, "/twitch/{type}", out var paramsPost))
            {
                await HandleTokenSubmit(ctx, paramsPost["type"]);
                return;
            }

            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        private static async Task ServeAuthPage(HttpListenerContext ctx)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(TwitchAuthTemplates.AuthPageHtml);
            ctx.Response.ContentType = "text/html";
            ctx.Response.ContentLength64 = buffer.Length;
            await ctx.Response.OutputStream.WriteAsync(buffer);
        }

        private async Task HandleTokenSubmit(HttpListenerContext ctx, string type)
        {
            try
            {
                using var reader = new StreamReader(ctx.Request.InputStream);
                string json = await reader.ReadToEndAsync();

                var payload = JsonSerializer.Deserialize<TwitchAuthResponse>(json);
                if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
                {
                    throw new Exception("Invalid payload or missing access token.");
                }

                var scopesList = (payload.Scope ?? "")
                    .Split([' ', '+'], StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                string tokenType = string.IsNullOrEmpty(payload.TokenType)
                    ? "Bearer"
                    : char.ToUpper(payload.TokenType[0]) + payload.TokenType.Substring(1);

                var authContext = new TwitchAuthContext(payload.AccessToken, tokenType, scopesList);
                string state = payload.State ?? string.Empty;

                if (type.ToLower() == "broadcaster")
                {
                    await _gateway.Broadcaster.OnLoginSuccess(authContext, state);
                }
                else if (type.ToLower() == "bot")
                {
                    await _gateway.Bot.OnLoginSuccess(authContext, state);
                }

                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                byte[] response = Encoding.UTF8.GetBytes("Data received");
                await ctx.Response.OutputStream.WriteAsync(response);
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                byte[] error = Encoding.UTF8.GetBytes(ex.Message);
                await ctx.Response.OutputStream.WriteAsync(error);
            }
        }
    }
}