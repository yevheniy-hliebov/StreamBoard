using System.Net;

namespace StreamTabula.Features.Servers.Services
{
    public interface IHttpController
    {
        string RoutePrefix { get; }
        Task HandleAsync(HttpListenerContext ctx);
    }
}
