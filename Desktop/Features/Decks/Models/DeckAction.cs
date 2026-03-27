using StreamBoard.Components.Controls;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Integrations.Common.Models;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    //[ActionCategory("Twitch", IntegrationIconType.Twitch)]
    [ActionCategory("System", FluentIconType.System)]
    public abstract class SystemDeckAction : DeckAction { }

    [ActionCategory("Input", FluentIconType.Rename)]
    public abstract class InputDeckAction : DeckAction { }

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
