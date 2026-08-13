using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using System.Diagnostics;
using System.Text.Json.Serialization;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("website")]
public class WebsiteAction : SystemBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Open Website",
        DialogTitle: "Enter URL",
        Icon: FluentIconType.Globe
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

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

    [JsonIgnore]
    public override string Label => string.IsNullOrEmpty(Url) ? Metadata.Name : $"{Metadata.Name} ({Url})";

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

    public override BaseAction Copy() => new WebsiteAction
    {
        Id = this.Id,
        Url = this.Url
    };
}
