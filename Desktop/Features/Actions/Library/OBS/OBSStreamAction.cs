using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.OBS;

[ActionDiscriminator("obs_stream")]
[ActionInfo("Stream", "Stream Settings", FluentIconType.Streaming)]
public class OBSStreamAction : OBSBaseAction
{
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
        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();
        if (!obs.IsConnected) return;

        try
        {
            switch (StreamState)
            {
                case "Start":
                    obs.StartStream();
                    break;
                case "Stop":
                    obs.StopStream();
                    break;
                case "Toggle":
                default:
                    obs.ToggleStream();
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OBS Stream] {ex.Message}");
        }

        await Task.CompletedTask;
    }
}