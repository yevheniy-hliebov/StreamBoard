using StreamBoard.Core;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckProfile<TCanvasConfig> : ObservableObject where TCanvasConfig : new()
    {
        [JsonPropertyName("pages")]
        public DeckPagesState PagesState { get; set; } = new();

        [JsonPropertyName("page_and_button_map")]
        public Dictionary<string, Dictionary<string, DeckButtonConfig>> ButtonMaps { get; set; } = [];

        [JsonPropertyName("canvas_config")]
        public TCanvasConfig CanvasConfig { get; set; } = new();

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
