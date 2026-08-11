using StreamTabula.Core.Mvvm;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckPagesState : ObservableObject
    {
        private string _selectedPageId = "";

        [JsonPropertyName("selected_page")]
        public string SelectedPageId
        {
            get => _selectedPageId;
            set => SetProperty(ref _selectedPageId, value);
        }

        [JsonPropertyName("list")]
        public ObservableCollection<DeckPage> AllPages { get; set; } = [];
    }
}
