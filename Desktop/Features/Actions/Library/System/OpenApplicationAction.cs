using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("open_application")]
[ActionInfo("Open Application", "Enter Application Path", FluentIconType.Apps)]
public class OpenApplicationAction : SystemBaseAction
{
    private string _appPath = "";

    [PathField("Application Path", PathSelectionType.File, Filter = "Executables (*.exe)|*.exe|Shortcuts (*.lnk)|*.lnk|All files (*.*)|*.*", Hint = "Select .exe or shortcut...")]
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
}