using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("website")]
    public class WebsiteAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open Website",
            DialogTitle: "Enter URL",
            Icon: FluentIconType.Globe
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _url = "";

        [JsonPropertyName("url")]
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(Url) ? Metadata.Name : $"{Metadata.Name} ({Url})";

        public override Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(Url)) return Task.CompletedTask;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not open link: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public override DeckAction Copy() => new WebsiteAction { Url = Url };
    }
}
