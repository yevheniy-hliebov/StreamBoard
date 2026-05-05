using StreamTabula.Core;
using StreamTabula.Features.Actions.Attributes;
using System.Reflection;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Models
{
    public abstract class BaseAction : ObservableObject
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = Guid.NewGuid().ToString();


        [JsonIgnore]
        public abstract ActionMetadata Metadata { get; }

        [JsonIgnore]
        public string CategoryName
        {
            get
            {
                var categoryAttribute = GetType().GetCustomAttribute<ActionCategoryAttribute>(true);
                return categoryAttribute?.Name ?? "Uncategorized";
            }
        }

        [JsonIgnore]
        public virtual string Label => Metadata.Name;

        [JsonIgnore]
        public string FullLabel => $"{CategoryName} | {Label}";

        public abstract Task ExecuteAsync(object? data = null);

        public abstract BaseAction Copy();
    }
}
