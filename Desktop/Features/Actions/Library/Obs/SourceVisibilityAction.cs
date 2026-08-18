using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Integrations.Obs.Services;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Obs;

[ActionDiscriminator("obs_source_visibility")]
public class SourceVisibilityAction : ObsBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Source Visibility",
        DialogTitle: "Source Visibility Settings",
        Icon: FluentIconType.View
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

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

    [JsonIgnore]
    public override string Label
    {
        get
        {
            if (string.IsNullOrEmpty(SceneName) || string.IsNullOrEmpty(SourceName))
            {
                return Metadata.Name;
            }

            return $"{Metadata.Name} ({SceneName}, {SourceName}, {VisibilityState})";
        }
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

    public override BaseAction Copy() => new SourceVisibilityAction
    {
        Id = this.Id,
        SceneName = this.SceneName,
        SourceName = this.SourceName,
        VisibilityState = this.VisibilityState
    };
}