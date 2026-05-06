using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using System.Net;
using System.Text;

namespace StreamTabula.Features.Servers.Controllers
{
    public class HomeController(LocalServerConfig config) : IHttpController
    {
        private readonly LocalServerConfig _config = config;

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
