using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    [ActionDiscriminator("obs_mute_source")]
    public class MuteSourceAction : ObsDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Mute Source",
            DialogTitle: "Mute Source Settings",
            Icon: FluentIconType.Apps
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _sceneName = string.Empty;
        private string _sourceName = string.Empty;
        private string _muteState = "Toggle";

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

        [ActionSetting("State", "Select state...", typeof(ObsMuteStateOptionsProvider))]
        [JsonPropertyName("mute_state")]
        public string MuteState
        {
            get => _muteState;
            set => SetProperty(ref _muteState, value);
        }

        public MuteSourceAction()
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
            if (string.IsNullOrWhiteSpace(SourceName)) return;

            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                switch (MuteState)
                {
                    case "Mute":
                        obsService.Obs.SetInputMute(SourceName, true);
                        break;
                    case "Unmute":
                        obsService.Obs.SetInputMute(SourceName, false);
                        break;
                    case "Toggle":
                    default:
                        obsService.Obs.ToggleInputMute(SourceName);
                        break;
                }
            }
            catch (Exception ex) { Debug.WriteLine($"[OBS Mute] {ex.Message}"); }

            await Task.CompletedTask;
        }

        public override DeckAction Copy() => new MuteSourceAction
        {
            SceneName = SceneName,
            SourceName = SourceName,
            MuteState = MuteState
        };
    }
}