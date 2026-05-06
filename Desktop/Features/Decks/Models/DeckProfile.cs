using StreamTabula.Core;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckProfile : ObservableObject
    {
        [JsonPropertyName("pages")]
        public DeckPagesState PagesState { get; set; } = new();

        [JsonPropertyName("page_and_button_map")]
        public Dictionary<string, Dictionary<string, DeckButtonConfig>> ButtonMaps { get; set; } = [];

        [JsonPropertyName("canvas_config")]
        public BaseCanvasConfig CanvasConfig { get; set; } = null!;

        [JsonIgnore]
        public Dictionary<string, DeckButtonConfig> CurrentPageButtonMap // TODO: Перенести в інше місце, або відмовитись
        {
            get
            {
                string selectedPageId = PagesState.SelectedPageId ?? "";

                if (!ButtonMaps.TryGetValue(selectedPageId, out var map))
                {
                    map = new Dictionary<string, DeckButtonConfig>();
                    ButtonMaps[selectedPageId] = map;
                }

                return map;
            }
        }
    }
}
