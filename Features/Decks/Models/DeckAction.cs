using StreamBoard.Core;
using StreamBoard.Features.Decks.Attributes;
using System.Text.Json.Serialization;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.Models
{
    //[ActionCategory("System", IconPath = "/Assets/Images/Integrations/twitch.png")]
    [ActionCategory("System", SymbolRegular.Laptop24)]
    public abstract class SystemDeckAction : DeckAction { }

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
