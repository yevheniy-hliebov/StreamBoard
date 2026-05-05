using StreamTabula.Core;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public enum DeckType
    {
        Grid,
        Keyboard
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(GridCanvasConfig), typeDiscriminator: "grid")]
    [JsonDerivedType(typeof(KeyboardCanvasConfig), typeDiscriminator: "keyboard")]
    public abstract class BaseCanvasConfig(DeckType type) : ObservableObject
    {
        public readonly DeckType Type = type;
    }
}
