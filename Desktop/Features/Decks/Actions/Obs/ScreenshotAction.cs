using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Helpers;

namespace StreamBoard.Features.Decks.Actions.Obs
{
    [ActionDiscriminator("obs_screenshot")]
    public class ScreenshotAction : ObsDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Screenshot",
            DialogTitle: "Screenshot Settings",
            Icon: FluentIconType.RectangularClipping
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private bool _captureActiveScene = true;
        private string _sceneName = string.Empty;
        private string _savePath = string.Empty;
        private bool _playSoundOnComplete = false;

        [ActionSetting("Capture Active Scene", "If true, takes a screenshot of the current live scene.")]
        [JsonPropertyName("capture_active_scene")]
        public bool CaptureActiveScene
        {
            get => _captureActiveScene;
            set
            {
                if (SetProperty(ref _captureActiveScene, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [ActionSetting("Scene", "Select scene if not capturing active...", typeof(ObsSceneOptionsProvider))]
        [JsonPropertyName("scene_name")]
        public string SceneName
        {
            get => _sceneName;
            set
            {
                if (SetProperty(ref _sceneName, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [ActionSetting("Save Path", "Directory path to save the screenshot.")]
        [JsonPropertyName("save_path")]
        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        [ActionSetting("Play Sound", "Play a notification sound when done.")]
        [JsonPropertyName("play_sound_on_complete")]
        public bool PlaySoundOnComplete
        {
            get => _playSoundOnComplete;
            set => SetProperty(ref _playSoundOnComplete, value);
        }

        [ActionSetting("Sound Volume", "Volume level (0-100%)")]
        [JsonPropertyName("sound_volume")]
        public int SoundVolume { get; set; } = 50;

        [JsonIgnore]
        public override string Label
        {
            get
            {
                string target = CaptureActiveScene ? "Live Scene" : (string.IsNullOrEmpty(SceneName) ? "Unknown" : SceneName);
                return $"{Metadata.Name} ({target})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            var obsService = App.ServiceProvider.GetRequiredService<ObsService>();
            if (!obsService.IsConnected) return;

            try
            {
                string targetScene = CaptureActiveScene ? obsService.Obs.GetCurrentProgramScene() : SceneName;

                if (string.IsNullOrWhiteSpace(targetScene))
                {
                    Debug.WriteLine("[OBS Screenshot] Error: Scene not selected or unavailable.");
                    return;
                }

                string base64Data = obsService.Obs.GetSourceScreenshot(targetScene, "png");

                if (!string.IsNullOrEmpty(base64Data) && base64Data.Contains(","))
                {
                    string cleanBase64 = base64Data.Substring(base64Data.IndexOf(',') + 1);
                    byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                    string fileName = $"OBS_{targetScene}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

                    string directory = string.IsNullOrWhiteSpace(SavePath)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                        : SavePath;

                    Directory.CreateDirectory(directory);

                    string fullPath = Path.Combine(directory, fileName);
                    await File.WriteAllBytesAsync(fullPath, imageBytes);

                    Debug.WriteLine($"[OBS Screenshot] Saved: {fullPath}");

                    if (PlaySoundOnComplete)
                    {
                        AudioPlayerService.Play("Assets/Sounds/shutter.mp3", SoundVolume);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OBS Screenshot] {ex.Message}");
            }
        }

        public override DeckAction Copy() => new ScreenshotAction
        {
            Id = this.Id,
            CaptureActiveScene = CaptureActiveScene,
            SceneName = SceneName,
            SavePath = SavePath,
            PlaySoundOnComplete = PlaySoundOnComplete
        };
    }
}