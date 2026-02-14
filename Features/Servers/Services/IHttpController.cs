using System.Net;

namespace StreamBoard.Features.Servers.Services
{
    public interface IHttpController
    {
        bool CanHandle(string path);
        Task HandleAsync(HttpListenerContext ctx);
    }
}
