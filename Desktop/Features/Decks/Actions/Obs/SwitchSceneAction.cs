using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    [ActionDiscriminator("obs_switch_scene")]
    public class SwitchSceneAction : ObsDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Switch Scene",
            DialogTitle: "Select OBS Scene",
            Icon: FluentIconType.FitPage
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _sceneName = string.Empty;

        [ActionSetting("Select Scene", "Choose a scene from the list...", typeof(ObsSceneOptionsProvider))]
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
                var obsService = App.ServiceProvider.GetRequiredService<ObsService>();

                if (obsService.IsConnected)
                {
                    obsService.Obs.SetCurrentProgramScene(SceneName);
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

        public override DeckAction Copy() => new SwitchSceneAction
        {
            Id = this.Id,
            SceneName = this.SceneName
        };
    }
}