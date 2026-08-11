using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Actions.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckButtonConfig : ObservableObject
    {
        private const string DefaultBackgroundColor = "#FF2D2D2D";

        private string _name = string.Empty;
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _backgroundColor = DefaultBackgroundColor;
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
        public ObservableCollection<BaseAction> Actions { get; set; } = [];

        [JsonIgnore]
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        [JsonIgnore]
        public bool HasData => Actions.Count > 0 ||
                               !string.IsNullOrWhiteSpace(Name) ||
                               !string.IsNullOrWhiteSpace(ImagePath) ||
                               BackgroundColor != DefaultBackgroundColor;

        public void ResetAppearance()
        {
            Name = string.Empty;
            BackgroundColor = "#FF2D2D2D";
            ImagePath = null;
        }
    }
}
