using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models.OBS;
using StreamTabula.Features.Integrations.OBS.Services;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_source_visibility")]
[ActionInfo("Source Visibility", "Source Visibility Settings", FluentIconType.View)]
public class SourceVisibilityAction : ObsBaseAction, IHasSceneName
{
    private string _sceneName = string.Empty;
    private string _sourceName = string.Empty;
    private string _visibilityState = "Toggle";

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

    [DropdownField("State", typeof(ObsVisibilityStateOptionsProvider), Hint = "Select state...")]
    [JsonPropertyName("visibility_state")]
    public string VisibilityState
    {
        get => _visibilityState;
        set => SetProperty(ref _visibilityState, value);
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(SceneName) || string.IsNullOrWhiteSpace(SourceName)) return;

        var obs = App.ServiceProvider.GetRequiredService<IOBSWebsocket>();
        if (!obs.IsConnected) return;

        try
        {
            var sceneService = App.ServiceProvider.GetRequiredService<IOBSSceneService>();
            var sourceInfo = sceneService.GetSourceInfo(SceneName, SourceName);

            if (sourceInfo == null)
            {
                Debug.WriteLine($"[OBS Visibility] Джерело '{SourceName}' не знайдено у '{SceneName}'.");
                return;
            }

            bool visibility = false;

            if (VisibilityState == "Show")
            {
                visibility = true;
            }
            else if (VisibilityState == "Toggle")
            {
                visibility = obs.GetSceneItemEnabled(sourceInfo.ParentName, sourceInfo.Id);
            }

            obs.SetSceneItemEnabled(sourceInfo.ParentName, sourceInfo.Id, visibility);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OBS Visibility] {ex.Message}");
        }

        await Task.CompletedTask;
    }
}