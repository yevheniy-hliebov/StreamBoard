using Microsoft.AspNetCore.Http;
using System.Net;

namespace StreamTabula.Features.Servers.Models;

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
        if (ip == "::1")
        {
            return IPAddress.Loopback.ToString();
        }

        if (ip?.Contains("::ffff:") ?? false)
        {
            ip = ip.Replace("::ffff:", "");
        }

        return ip ?? "Unknown";
    }
}