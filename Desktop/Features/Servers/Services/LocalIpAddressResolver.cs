using System.Net;
using System.Net.Sockets;

namespace StreamTabula.Features.Servers.Services;

public interface ILocalIpAddressResolver
{
    IPAddress Get();
}

public class LocalIpAddressResolver : ILocalIpAddressResolver
{
    public IPAddress Get()
    {
		try
		{
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localIp = host.AddressList.FirstOrDefault(ip =>
                ip.AddressFamily == AddressFamily.InterNetwork &&
                !IPAddress.IsLoopback(ip));

            return localIp ?? IPAddress.Loopback;
        }
		catch (Exception)
		{
            return IPAddress.Loopback;
		}
    }
}
