using System.Text.Json.Serialization;

namespace StreamTabula.Features.Variables.Models
{
    public class VariablesConfig
    {
        [JsonPropertyName("global_variables")]
        public List<Variable> GlobalVariables { get; set; } = [];
    }
}