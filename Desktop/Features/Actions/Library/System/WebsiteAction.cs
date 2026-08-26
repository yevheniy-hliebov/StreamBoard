using StreamTabula.Features.Actions.Attributes;
using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("website")]
[ActionInfo("Open Website", "Enter URL", FluentIconType.Globe)]
public class WebsiteAction : SystemBaseAction
{
    private string _url = "";

    [InputField("Website URL", Hint = "Enter url...")]
    [JsonPropertyName("url")]
    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Label));
        }
    }

    public override Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(Url)) return Task.CompletedTask;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = Url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not open link: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
