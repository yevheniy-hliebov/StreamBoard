namespace StreamBoard.Features.Servers.Models
{
    public class HttpServerConfig
    {
        public string Address { get; set; } = "localhost";

        public int Port { get; set; } = 13550;

        public bool AutoStart { get; set; } = false;

        public string Prefix => $"http://{Address}:{Port}/";
    }
}
