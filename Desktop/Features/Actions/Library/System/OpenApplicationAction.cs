using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;

namespace StreamBoard.Features.Actions.Library.System
{
    [ActionDiscriminator("open_application")]
    public class OpenApplicationAction : SystemBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open Application",
            DialogTitle: "Enter Application Path",
            Icon: FluentIconType.Apps
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _appPath = "";

        [InputField("Application Path", Hint = "Enter .exe or shortcut path...")]
        [JsonPropertyName("app_path")]
        public string AppPath
        {
            get => _appPath;
            set
            {
                _appPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        private string _arguments = "";

        [InputField("Arguments", Hint = "Enter startup arguments (optional)...")]
        [JsonPropertyName("arguments")]
        public string Arguments
        {
            get => _arguments;
            set
            {
                _arguments = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(AppPath)
            ? Metadata.Name : $"{Metadata.Name} ({Path.GetFileNameWithoutExtension(AppPath)})";

        public override Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(AppPath)) return Task.CompletedTask;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = AppPath,
                    Arguments = Arguments,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not open application: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new OpenApplicationAction
        {
            Id = this.Id,
            AppPath = this.AppPath,
            Arguments = this.Arguments
        };
    }
}