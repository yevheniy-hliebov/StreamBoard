using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;
using OBSWebsocketDotNet;

namespace StreamTabula.Features.Actions.Library.OBS;

[ActionDiscriminator("obs_mute_source")]
[ActionInfo("Mute Source", "Mute Source Settings", FluentIconType.Mute)]
public class OBSMuteSourceAction : OBSBaseAction, IHasSceneName
{
    private string _sceneName = string.Empty;
    private string _sourceName = string.Empty;
    private string _muteState = "Toggle";

    [DropdownField("Scene", typeof(OBSSceneOptionsProvider), Hint = "Select scene...")]
    [JsonPropertyName("scene_name")]
    public string SceneName
    {
        get => _sceneName;
        set => SetProperty(ref _sceneName, value);
    }

    [DropdownField("Source", typeof(ObsSourceOptionsProvider), Hint = "Select source...")]
    [JsonPropertyName("source_name")]
    public string SourceName
    {
        get => _sourceName;
        set => SetProperty(ref _sourceName, value);
    }

    [DropdownField("State", typeof(ObsMuteStateOptionsProvider), Hint = "Select state...")]
    [JsonPropertyName("mute_state")]
    public string MuteState
    {
        get => _muteState;
        set => SetProperty(ref _muteState, value);
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(SourceName)) return;

        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();
        if (!obs.IsConnected) return;

        try
        {
            switch (MuteState)
            {
                case "Mute":
                    obs.SetInputMute(SourceName, true);
                    break;
                case "Unmute":
                    obs.SetInputMute(SourceName, false);
                    break;
                case "Toggle":
                default:
                    obs.ToggleInputMute(SourceName);
                    break;
            }
        }
        catch (Exception ex) { Debug.WriteLine($"[OBS Mute] {ex.Message}"); }

        await Task.CompletedTask;
    }
}