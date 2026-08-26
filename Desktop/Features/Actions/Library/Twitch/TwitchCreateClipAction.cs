using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_create_clip")]
[ActionInfo("Create Clip", "Create Clip Settings", FluentIconType.Video)]
public class TwitchCreateClipAction : TwitchBaseAction
{
    private string _clipTitle = string.Empty;
    private int _durationSeconds = 0;

    [InputField("Title", Hint = "Optional. Title for the clip.")]
    [JsonPropertyName("clip_title")]
    public string ClipTitle
    {
        get => _clipTitle;
        set
        {
            if (SetProperty(ref _clipTitle, value))
                OnPropertyChanged(nameof(Label));
        }
    }

    [InputField("Duration", Hint = "Optional. Duration in seconds (0 = default).")]
    [JsonPropertyName("duration_seconds")]
    public int DurationSeconds
    {
        get => _durationSeconds;
        set
        {
            int val = value < 0 ? 0 : value;
            SetProperty(ref _durationSeconds, val);
        }
    }

    [JsonIgnore]
    public override string Label
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ClipTitle)) return Metadata.Name;
            return $"{Metadata.Name} ({ClipTitle})";
        }
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (!broadcaster.Session.IsAuthenticated || broadcaster.Session.User?.Id == null)
            {
                return;
            }

            string? finalTitle = string.IsNullOrWhiteSpace(ClipTitle) ? null : ClipTitle;
            float? finalDuration = DurationSeconds > 0 ? DurationSeconds : null;

            await broadcaster.Api.Production.CreateClip(
                broadcaster.Session.User.Id,
                finalTitle,
                finalDuration
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Twitch Create Clip] Error: {ex.Message}");
        }
    }
}