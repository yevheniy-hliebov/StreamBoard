using Microsoft.AspNetCore.Http;

namespace StreamTabula.Features.Servers.Services
{
    public interface IHttpController
    {
        string RoutePrefix { get; }
        Task HandleAsync(HttpContext ctx);
    }
}