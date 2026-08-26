using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_set_steam_title")]
[ActionInfo("Set Stream Title", "Set Stream Title Settings", FluentIconType.Edit)]
public class TwitchSetStreamTitleAction : TwitchBaseAction
{
    private string _title = string.Empty;

    [InputField("Title", Hint = "Enter title...")]
    [JsonPropertyName("title")]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Label));
        }
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(Title)) return;

        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (broadcaster.Session.IsAuthenticated && broadcaster.Session.User != null)
            {
                string? broadcasterId = gateway.Broadcaster.Session.User?.Id;

                if (broadcasterId != null)
                {
                    await broadcaster.Api.Channel.UpdateTitle(broadcasterId, Title);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Twitch set stream title error: {ex.Message}");
            throw;
        }
    }
}