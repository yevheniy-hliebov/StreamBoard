using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("open_folder")]
[ActionInfo("Open Folder", "Enter Folder Path", FluentIconType.Folder)]
public class OpenFolderAction : SystemBaseAction
{
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
}