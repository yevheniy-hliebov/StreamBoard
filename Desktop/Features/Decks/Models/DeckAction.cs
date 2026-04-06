using StreamBoard.Core;
using StreamBoard.Features.Decks.Attributes;
using System.Reflection;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public abstract class DeckAction : ObservableObject
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

        public abstract DeckAction Copy();
    }
}
