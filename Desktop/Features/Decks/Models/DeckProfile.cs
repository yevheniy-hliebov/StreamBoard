using StreamBoard.Core;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckProfile<TCanvasConfig> : ObservableObject where TCanvasConfig : new()
    {
        [JsonPropertyName("pages")]
        public DeckPagesConfig Pages { get; set; } = new();

        [JsonPropertyName("page_and_button_map")]
        public Dictionary<string, Dictionary<string, DeckButtonConfig>> PageButtonMap { get; set; } = [];

        [JsonPropertyName("canvas_config")]
        public TCanvasConfig CanvasConfig { get; set; } = new();

        [JsonIgnore]
        public Dictionary<string, DeckButtonConfig> CurrentPageButtonMap
        {
            get
            {
                string selectedPageId = Pages.SelectedPageId ?? "";

                if (!PageButtonMap.TryGetValue(selectedPageId, out var map))
                {
                    map = new Dictionary<string, DeckButtonConfig>();
                    PageButtonMap[selectedPageId] = map;
                }

                return map;
            }
        }
    }

    public class DeckPagesConfig : ObservableObject
    {
        private string _selectedPageId = "";

        [JsonPropertyName("selected_page")]
        public string SelectedPageId
        {
            get => _selectedPageId;
            set => SetProperty(ref _selectedPageId, value);
        }

        [JsonPropertyName("list")]
        public ObservableCollection<DeckPageInfo> List { get; set; } = [];
    }

    public class DeckPageInfo : ObservableObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        private string _name = "New Page";

        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
