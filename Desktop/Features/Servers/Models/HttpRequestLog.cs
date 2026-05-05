using System.Net;

namespace StreamTabula.Features.Servers.Models
{
    public record HttpRequestLog(string Method, string Endpoint, string IpAddress, int StatusCode, DateTime Timestamp)
    {
        public HttpRequestLog(HttpListenerContext ctx) : this(
            ctx.Request.HttpMethod,
            ctx.Request.Url?.AbsolutePath ?? "/",
            (ctx.Request.RemoteEndPoint?.Address.ToString() == "::1" ? "127.0.0.1" : ctx.Request.RemoteEndPoint?.Address.ToString()) ?? "Unknown",
            ctx.Response.StatusCode,
            DateTime.Now
        )
        { }
    }
}
