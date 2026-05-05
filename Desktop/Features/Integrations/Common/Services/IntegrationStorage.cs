using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Common.Models;

namespace StreamTabula.Features.Integrations.Common.Services
{
    public class IntegrationConnectionStorage : JsonFileStorage<IntegrationConnectionSettings>
    {
        public IntegrationConnectionStorage() : base("integraions.json") { }
    }
}