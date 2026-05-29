using System.Text.Json.Serialization;
using StreamTabula.Core;

namespace StreamTabula.Features.Variables.Models
{
    public enum VariableScope { Runtime, Temporary, Global }

    public class Variable : ObservableObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("scope")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VariableScope Scope { get; set; }

        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        private string _value = string.Empty;

        [JsonPropertyName("value")]
        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value ?? string.Empty);
        }
    }
}