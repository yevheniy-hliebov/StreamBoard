using Microsoft.AspNetCore.Http;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using System.Net;

namespace StreamTabula.Features.Servers.Controllers
{
    public class HomeController(LocalServerConfig config) : IHttpController
    {
        private readonly LocalServerConfig _config = config;

        public string RoutePrefix => "/";

        public async Task HandleAsync(HttpContext ctx)
        {
            try
            {
                var address = NetworkHelper.GetLocalIpAddress();
                string responseText = $"Running on {address}:{_config.Port}";

                ctx.Response.ContentType = "text/plain";
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                await ctx.Response.WriteAsync(responseText);
            }
            catch (Exception)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}