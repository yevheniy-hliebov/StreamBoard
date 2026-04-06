using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    [ActionDiscriminator("obs_source_visibility")]
    public class SourceVisibilityAction : ObsDeckAction
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

        [ActionSetting("Scene", "Select scene...", typeof(ObsSceneOptionsProvider))]
        [JsonPropertyName("scene_name")]
        public string SceneName
        {
            get => _sceneName;
            set => SetProperty(ref _sceneName, value);
        }

        [ActionSetting("Source", "Select source...", typeof(ObsSourceOptionsProvider))]
        [JsonPropertyName("source_name")]
        public string SourceName
        {
            get => _sourceName;
            set => SetProperty(ref _sourceName, value);
        }

        [ActionSetting("State", "Select state...", typeof(ObsVisibilityStateOptionsProvider))]
        [JsonPropertyName("visibility_state")]
        public string VisibilityState
        {
            get => _visibilityState;
            set => SetProperty(ref _visibilityState, value);
        }

        public SourceVisibilityAction()
        {
            var obs = App.ServiceProvider.GetService<ObsService>();
            if (obs?.IsConnected == true)
            {
                var scenes = obs.Obs.GetSceneList();
                var firstScene = scenes.Scenes.FirstOrDefault()?.Name;
                if (firstScene != null) _sceneName = firstScene;
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(SceneName) || string.IsNullOrWhiteSpace(SourceName)) return;

            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                var targetInfo = obsService.Obs.GetSourceParentAndId(SceneName, SourceName);

                if (targetInfo == null)
                {
                    Debug.WriteLine($"[OBS Visibility] Джерело '{SourceName}' не знайдено у '{SceneName}'.");
                    return;
                }

                string actualParent = targetInfo.Value.ParentScene;
                int itemId = targetInfo.Value.SourceId;

                switch (VisibilityState)
                {
                    case "Show":
                        obsService.Obs.SetSceneItemEnabled(actualParent, itemId, true);
                        break;
                    case "Hide":
                        obsService.Obs.SetSceneItemEnabled(actualParent, itemId, false);
                        break;
                    case "Toggle":
                    default:
                        bool isEnabled = obsService.Obs.GetSceneItemEnabled(actualParent, itemId);
                        obsService.Obs.SetSceneItemEnabled(actualParent, itemId, !isEnabled);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OBS Visibility] {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public override DeckAction Copy() => new SourceVisibilityAction
        {
            Id = this.Id,
            SceneName = SceneName,
            SourceName = SourceName,
            VisibilityState = VisibilityState
        };
    }
}