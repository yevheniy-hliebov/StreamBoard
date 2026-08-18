using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;
using OBSWebsocketDotNet;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_mute_source")]
public class MuteSourceAction : ObsBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Mute Source",
        DialogTitle: "Mute Source Settings",
        Icon: FluentIconType.Mute
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

    private string _sceneName = string.Empty;
    private string _sourceName = string.Empty;
    private string _muteState = "Toggle";

    [DropdownField("Scene", typeof(ObsSceneOptionsProvider), Hint = "Select scene...")]
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

    [JsonIgnore]
    public override string Label
    {
        get
        {
            if (string.IsNullOrEmpty(SceneName) || string.IsNullOrEmpty(SourceName))
            {
                return Metadata.Name;
            }

            return $"{Metadata.Name} ({SceneName}, {SourceName}, {MuteState})";
        }
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

    public override BaseAction Copy() => new MuteSourceAction
    {
        Id = this.Id,
        SceneName = this.SceneName,
        SourceName = this.SourceName,
        MuteState = this.MuteState
    };
}