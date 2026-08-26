using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models.OBS;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_switch_scene")]
[ActionInfo("Switch Scene", "Select OBS Scene", FluentIconType.FitPage)]
public class SwitchSceneAction : ObsBaseAction, IHasSceneName
{
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
}