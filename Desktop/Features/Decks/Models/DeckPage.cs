using StreamBoard.Core;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckPage : ObservableObject
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
