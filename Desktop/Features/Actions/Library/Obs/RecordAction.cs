using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Obs.Services;

namespace StreamTabula.Features.Actions.Library.Obs
{
    [ActionDiscriminator("obs_record")]
    public class RecordAction : ObsBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Record",
            DialogTitle: "Record Settings",
            Icon: FluentIconType.Record
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _recordState = "Toggle";

        [DropdownField("State", typeof(ObsRecordStateOptionsProvider), Hint = "Select record state...")]
        [JsonPropertyName("record_state")]
        public string RecordState
        {
            get => _recordState;
            set => SetProperty(ref _recordState, value);
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                return $"{Metadata.Name} ({RecordState})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                switch (RecordState)
                {
                    case "Start":
                        obsService.Obs.StartRecord();
                        break;
                    case "Stop":
                        obsService.Obs.StopRecord();
                        break;
                    case "Pause":
                        obsService.Obs.PauseRecord();
                        break;
                    case "Resume":
                        obsService.Obs.ResumeRecord();
                        break;
                    case "Toggle Pause":
                        obsService.Obs.ToggleRecordPause();
                        break;
                    case "Toggle":
                    default:
                        obsService.Obs.ToggleRecord();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OBS Record] {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public override BaseAction Copy() => new RecordAction
        {
            Id = this.Id,
            RecordState = this.RecordState
        };
    }
}