using StreamBoard.Features.Servers.Models;
using StreamBoard.Features.Servers.Services;
using System.Net;
using System.Text;

namespace StreamBoard.Features.Servers.Controllers
{
    public class HomeController : IHttpController
    {
        private readonly HttpServerConfig _config;

        public HomeController(HttpServerConfig config)
        {
            _config = config;
        }

        public string RoutePrefix => "/";

        public async Task HandleAsync(HttpListenerContext ctx)
        {
            try
            {
                string responseText = $"Running on {_config.Address}:{_config.Port}";
                byte[] data = Encoding.UTF8.GetBytes(responseText);

                ctx.Response.ContentType = "text/plain";
                ctx.Response.ContentLength64 = data.Length;
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;

                await ctx.Response.OutputStream.WriteAsync(data);
            }
            catch (Exception)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
