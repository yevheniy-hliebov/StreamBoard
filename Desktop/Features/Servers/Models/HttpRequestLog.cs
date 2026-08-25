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
        var ip = ctx.Connection.RemoteIpAddress;

        if (ip == null)
            return "Unknown";

        if (ip.Equals(IPAddress.IPv6Loopback))
        {
            return IPAddress.Loopback.ToString();
        }

        if (ip.IsIPv4MappedToIPv6)
        {
            return ip.MapToIPv4().ToString();
        }

        return ip.ToString();
    }
}