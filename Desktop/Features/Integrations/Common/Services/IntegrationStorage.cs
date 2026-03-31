using StreamBoard.Core.Services;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Integrations.Common.Services
{
    public class IntegrationConnectionStorage : JsonFileStorage<IntegrationConnectionSettings>
    {
        public IntegrationConnectionStorage() : base("integraions.json") { }
    }
}