using StreamTabula.Core.Models;
using StreamTabula.Core.Mvvm;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckProfile : ObservableObject, IVersionedConfig
    {
        [JsonPropertyName("config_version")]
        public int ConfigVersion { get; set; } = 1;

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
