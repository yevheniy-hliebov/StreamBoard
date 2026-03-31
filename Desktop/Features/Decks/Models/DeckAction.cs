using StreamBoard.Components.Controls;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Integrations.Common.Models;
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
        public virtual string Label => Metadata.Name;

        public abstract Task ExecuteAsync(object? data = null);

        public abstract DeckAction Copy();
    }
}
