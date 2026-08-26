using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("open_file")]
[ActionInfo("Open File", "Enter File Path", FluentIconType.Document)]
public class OpenFileAction : SystemBaseAction
{
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
}