using StreamBoard.Core;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckProfile : ObservableObject
    {
        [JsonPropertyName("pages")]
        public DeckPagesConfig Pages { get; set; } = new();

        [JsonPropertyName("page_and_button_map")]
        public Dictionary<string, Dictionary<string, DeckButton>> PageButtonMap { get; set; } = [];
    }

    public class DeckPagesConfig : ObservableObject
    {
        private string _selectedPageId = "";

        [JsonPropertyName("selected")]
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
