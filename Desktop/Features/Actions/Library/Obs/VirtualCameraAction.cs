using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

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

    [DropdownField("State", typeof(ObsOutputStateOptionsProvider), Hint = "Select camera state...")]
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
        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();
        if (!obs.IsConnected) return;

        try
        {
            switch (CameraState)
            {
                case "Start":
                    obs.StartVirtualCam();
                    break;
                case "Stop":
                    obs.StopVirtualCam();
                    break;
                case "Toggle":
                default:
                    obs.ToggleVirtualCam();
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
        CameraState = this.CameraState
    };
}