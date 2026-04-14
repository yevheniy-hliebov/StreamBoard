using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("open_folder")]
    public class OpenFolderAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open Folder",
            DialogTitle: "Enter Folder Path",
            Icon: FluentIconType.Folder
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _folderPath = "";

        [ActionSetting("Folder Path", "Enter folder path...")]
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

        public override DeckAction Copy() => new OpenFolderAction
        {
            Id = this.Id,
            FolderPath = this.FolderPath
        };
    }
}