using StreamBoard.Core.Services;
using StreamBoard.Features.Servers.Models;

namespace StreamBoard.Features.Servers.Services
{
    public class ServerConfigsStorage : JsonFileStorage<ServerConfigs>
    {
        public ServerConfigsStorage() : base("servers.json") { }
    }
}
