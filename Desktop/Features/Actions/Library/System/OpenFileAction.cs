using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;

namespace StreamTabula.Features.Actions.Library.System
{
    [ActionDiscriminator("open_file")]
    public class OpenFileAction : SystemBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Open File",
            DialogTitle: "Enter File Path",
            Icon: FluentIconType.Document
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _filePath = "";

        [PathField("File Path", PathSelectionType.File, Hint = "Enter file path...")]
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

        public override BaseAction Copy() => new OpenFileAction
        {
            Id = this.Id,
            FilePath = this.FilePath
        };
    }
}