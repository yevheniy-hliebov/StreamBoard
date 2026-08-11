using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Components.Enums;

namespace StreamTabula.Features.Actions.Library.System
{
    [ActionDiscriminator("open_folder")]
    public class OpenFolderAction : SystemBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open Folder",
            DialogTitle: "Enter Folder Path",
            Icon: FluentIconType.Folder
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _folderPath = "";

        [PathField("Folder Path", PathSelectionType.Folder, Hint = "Enter folder path...")]
        [JsonPropertyName("folder_path")]
        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(FolderPath) ? Metadata.Name : $"{Metadata.Name} ({FolderPath})";

        public override Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(FolderPath)) return Task.CompletedTask;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = FolderPath,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not open folder: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new OpenFolderAction
        {
            Id = this.Id,
            FolderPath = this.FolderPath
        };
    }
}