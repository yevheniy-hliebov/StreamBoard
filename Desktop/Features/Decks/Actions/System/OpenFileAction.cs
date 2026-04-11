using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("open_file")]
    public class OpenFileAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open File",
            DialogTitle: "Enter File Path",
            Icon: FluentIconType.Document
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _filePath = "";

        [ActionSetting("File Path", "Enter file path...")]
        [JsonPropertyName("file_path")]
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => string.IsNullOrEmpty(FilePath) ? Metadata.Name : $"{Metadata.Name} ({FilePath})";

        public override Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(FilePath)) return Task.CompletedTask;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = FilePath,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not open file: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public override DeckAction Copy() => new OpenFileAction
        {
            Id = this.Id,
            FilePath = this.FilePath
        };
    }
}