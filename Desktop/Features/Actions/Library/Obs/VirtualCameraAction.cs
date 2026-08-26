using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_virtual_camera")]
[ActionInfo("Virtual Camera", "Virtual Camera Settings", FluentIconType.Video)]
public class VirtualCameraAction : ObsBaseAction
{
    private string _cameraState = "Toggle";

    [DropdownField("State", typeof(ObsOutputStateOptionsProvider), Hint = "Select camera state...")]
    [JsonPropertyName("camera_state")]
    public string CameraState
    {
        get => _cameraState;
        set => SetProperty(ref _cameraState, value);
    }

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
}