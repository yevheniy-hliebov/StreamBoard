using StreamTabula.Core.Services;
using StreamTabula.Features.Variables.Models;

namespace StreamTabula.Features.Variables.Services
{
    public class VariablesJsonStorage : JsonFileStorage<VariablesConfig>
    {
        public VariablesJsonStorage() : base("variables.json") { }
    }
}