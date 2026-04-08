using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    [ActionDiscriminator("obs_stream")]
    public class StreamAction : ObsDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Stream",
            DialogTitle: "Stream Settings",
            Icon: FluentIconType.Record
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _streamState = "Toggle";

        [ActionSetting("State", "Select stream state...", typeof(ObsStreamStateOptionsProvider))]
        [JsonPropertyName("stream_state")]
        public string StreamState
        {
            get => _streamState;
            set => SetProperty(ref _streamState, value);
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} ({StreamState})";

        public override async Task ExecuteAsync(object? data = null)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                switch (StreamState)
                {
                    case "Start":
                        obsService.Obs.StartStream();
                        break;
                    case "Stop":
                        obsService.Obs.StopStream();
                        break;
                    case "Toggle":
                    default:
                        obsService.Obs.ToggleStream();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OBS Stream] {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public override DeckAction Copy() => new StreamAction
        {
            Id = this.Id,
            StreamState = StreamState
        };
    }
}