using System.Net;

namespace StreamBoard.Features.Servers.Services
{
    public interface IHttpController
    {
        string RoutePrefix { get; }
        Task HandleAsync(HttpListenerContext ctx);
    }
}
