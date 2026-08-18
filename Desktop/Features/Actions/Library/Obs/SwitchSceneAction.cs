using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_switch_scene")]
public class SwitchSceneAction : ObsBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Switch Scene",
        DialogTitle: "Select OBS Scene",
        Icon: FluentIconType.FitPage
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

    private string _sceneName = string.Empty;

    [DropdownField("Select Scene", typeof(ObsSceneOptionsProvider), Hint = "Choose a scene from the list...")]
    [JsonPropertyName("scene_name")]
    public string SceneName
    {
        get => _sceneName;
        set
        {
            if (SetProperty(ref _sceneName, value))
            {
                OnPropertyChanged(nameof(Label));
            }
        }
    }

    [JsonIgnore]
    public override string Label => string.IsNullOrEmpty(SceneName)
        ? Metadata.Name
        : $"{Metadata.Name} ({SceneName})";

    public override async Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(SceneName)) return;

        try
        {
            var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();

            if (obs.IsConnected)
            {
                obs.SetCurrentProgramScene(SceneName);
            }
            else
            {
                Debug.WriteLine("[OBS Action] Cannot switch scene: OBS is not connected.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OBS Action] Error switching to scene '{SceneName}': {ex.Message}");
        }

        await Task.CompletedTask;
    }

    public override BaseAction Copy() => new SwitchSceneAction
    {
        Id = this.Id,
        SceneName = this.SceneName
    };
}