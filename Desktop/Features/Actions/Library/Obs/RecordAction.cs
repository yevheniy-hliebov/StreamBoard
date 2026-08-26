using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_record")]
[ActionInfo("Record", "Record Settings", FluentIconType.Record)]
public class RecordAction : ObsBaseAction
{
    private string _recordState = "Toggle";

    [DropdownField("State", typeof(ObsRecordStateOptionsProvider), Hint = "Select record state...")]
    [JsonPropertyName("record_state")]
    public string RecordState
    {
        get => _recordState;
        set => SetProperty(ref _recordState, value);
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();
        if (!obs.IsConnected) return;

        try
        {
            switch (RecordState)
            {
                case "Start":
                    obs.StartRecord();
                    break;
                case "Stop":
                    obs.StopRecord();
                    break;
                case "Pause":
                    obs.PauseRecord();
                    break;
                case "Resume":
                    obs.ResumeRecord();
                    break;
                case "Toggle Pause":
                    obs.ToggleRecordPause();
                    break;
                case "Toggle":
                default:
                    obs.ToggleRecord();
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OBS Record] {ex.Message}");
        }

        await Task.CompletedTask;
    }
}