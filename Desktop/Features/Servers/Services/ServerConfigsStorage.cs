using StreamTabula.Core.Services;
using StreamTabula.Features.Servers.Models;

namespace StreamTabula.Features.Servers.Services;

public class ServerConfigsStorage : JsonFileStorage<ServerConfigs>
{
    public ServerConfigsStorage() : base("servers.json") { }
}
