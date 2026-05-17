using Microsoft.AspNetCore.Http;
using System;

namespace StreamTabula.Features.Servers.Models
{
    public record HttpRequestLog(string Method, string Endpoint, string IpAddress, int StatusCode, DateTime Timestamp)
    {
        public HttpRequestLog(HttpContext ctx) : this(
            ctx.Request.Method,
            ctx.Request.Path.Value ?? "/",
            GetIpAddress(ctx),
            ctx.Response.StatusCode,
            DateTime.Now
        )
        { }

        private static string GetIpAddress(HttpContext ctx)
        {
            var ip = ctx.Connection.RemoteIpAddress?.ToString();
            return ip == "::1" ? "127.0.0.1" : (ip ?? "Unknown");
        }
    }
}