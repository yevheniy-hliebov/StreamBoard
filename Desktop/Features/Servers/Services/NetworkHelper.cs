using System.Net;
using System.Net.Sockets;

namespace StreamTabula.Features.Servers.Services
{
    public static class NetworkHelper
    {
        public static string GetLocalIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var localIp = host.AddressList.FirstOrDefault(ip =>
                    ip.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ip));

                return localIp?.ToString() ?? "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}
