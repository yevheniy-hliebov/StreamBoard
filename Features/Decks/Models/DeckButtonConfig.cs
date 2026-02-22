using StreamBoard.Core;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckButtonConfig : ObservableObject
    {
        private string _name = "";
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _backgroundColor = "#FF2D2D2D";
        [JsonPropertyName("background_color")]
        public string BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private string? _imagePath;
        [JsonPropertyName("image_path")]
        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        [JsonPropertyName("actions")]
        public ObservableCollection<DeckAction> Actions { get; set; } = [];

        [JsonIgnore]
        public bool HasName => !string.IsNullOrWhiteSpace(Name);
    }
}
