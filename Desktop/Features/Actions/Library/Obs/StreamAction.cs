using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Actions.Library.Obs
{
    [ActionDiscriminator("obs_stream")]
    public class StreamAction : ObsBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Stream",
            DialogTitle: "Stream Settings",
            Icon: FluentIconType.Streaming
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _streamState = "Toggle";

        [DropdownField("State", typeof(ObsOutputStateOptionsProvider), Hint = "Select stream state...")]
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

        public override BaseAction Copy() => new StreamAction
        {
            Id = this.Id,
            StreamState = this.StreamState
        };
    }
}