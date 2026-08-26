using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_create_stream_marker")]
[ActionInfo("Create Marker", "Stream Marker Settings", FluentIconType.Bookmarks)]
public class TwitchCreateStreamMarkerAction : TwitchBaseAction
{
    private string _description = string.Empty;

    [InputField("Description", Hint = "Optional. Max 140 chars.")]
    [JsonPropertyName("description")]
    public string Description
    {
        get => _description;
        set
        {
            string safeValue = value?.Length > 140 ? value[..140] : (value ?? string.Empty);

            if (SetProperty(ref _description, safeValue))
                OnPropertyChanged(nameof(Label));
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

            string? finalDescription = string.IsNullOrWhiteSpace(Description) ? null : Description;

            await broadcaster.Api.Production.CreateStreamMarker(
                broadcaster.Session.User.Id,
                finalDescription
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Twitch Stream Marker] Error: {ex.Message}");
        }
    }
}