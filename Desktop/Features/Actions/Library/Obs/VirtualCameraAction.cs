using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;

using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Actions.Library.Obs
{
    [ActionDiscriminator("obs_virtual_camera")]
    public class VirtualCameraAction : ObsBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Virtual Camera",
            DialogTitle: "Virtual Camera Settings",
            Icon: FluentIconType.Video
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _cameraState = "Toggle";

        [ActionSetting("State", "Select camera state...", typeof(ObsOutputStateOptionsProvider))]
        [JsonPropertyName("camera_state")]
        public string CameraState
        {
            get => _cameraState;
            set => SetProperty(ref _cameraState, value);
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} ({CameraState})";

        public override async Task ExecuteAsync(object? data = null)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                switch (CameraState)
                {
                    case "Start":
                        obsService.Obs.StartVirtualCam();
                        break;
                    case "Stop":
                        obsService.Obs.StopVirtualCam();
                        break;
                    case "Toggle":
                    default:
                        obsService.Obs.ToggleVirtualCam();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OBS VirtualCam] {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public override BaseAction Copy() => new VirtualCameraAction
        {
            Id = this.Id,
            CameraState = CameraState
        };
    }
}