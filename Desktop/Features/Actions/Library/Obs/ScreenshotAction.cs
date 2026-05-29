using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Helpers;

namespace StreamTabula.Features.Actions.Library.Obs
{
    [ActionDiscriminator("obs_screenshot")]
    public class ScreenshotAction : ObsBaseAction
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

        [InputField("Capture Active Scene", Hint = "If true, takes a screenshot of the current live scene.")]
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

        [DropdownField("Scene", typeof(ObsSceneOptionsProvider), Hint = "Select scene if not capturing active...")]
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

        [PathField("Save Path", PathSelectionType.Folder, Hint = "Directory path to save the screenshot.")]
        [JsonPropertyName("save_path")]
        public string SavePath
        {
            get => _savePath;
            set => SetProperty(ref _savePath, value);
        }

        [InputField("Play Sound", Hint = "Play a notification sound when done.")]
        [JsonPropertyName("play_sound_on_complete")]
        public bool PlaySoundOnComplete
        {
            get => _playSoundOnComplete;
            set => SetProperty(ref _playSoundOnComplete, value);
        }

        [InputField("Sound Volume", Hint = "Volume level (0-100%)")]
        [JsonPropertyName("sound_volume")]
        public int SoundVolume { get; set; } = 50;

        [JsonIgnore]
        public override string Label
        {
            get
            {
                string target = CaptureActiveScene ? "Active Scene" : (string.IsNullOrEmpty(SceneName) ? "Unknown" : SceneName);
                return $"{Metadata.Name} ({target})";
            }
        }

        public override async Task ExecuteAsync(ActionExecutionContext context)
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

        public override BaseAction Copy() => new ScreenshotAction
        {
            Id = this.Id,
            CaptureActiveScene = this.CaptureActiveScene,
            SceneName = this.SceneName,
            SavePath = this.SavePath,
            PlaySoundOnComplete = this.PlaySoundOnComplete
        };
    }
}