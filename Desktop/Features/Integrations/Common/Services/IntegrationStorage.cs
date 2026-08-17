using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Common.Models;

namespace StreamTabula.Features.Integrations.Common.Services;

public class IntegrationsStorage : JsonFileStorage<IntegrationsConfig>
{
    public IntegrationsStorage() : base("integraions.json") { }
}