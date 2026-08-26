using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_send_shoutout")]
[ActionInfo("Send Shoutout", "Send Shoutout Settings", FluentIconType.People)]
public class TwitchSendShoutoutAction : TwitchBaseAction
{
    private string _username = string.Empty;

    [InputField("Username", Hint = "Enter username...")]
    [JsonPropertyName("title")]
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Label));
        }
    }

    public override async Task ExecuteAsync(object? data = null)
    {
        if (string.IsNullOrWhiteSpace(Username)) return;

        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (broadcaster.Session.IsAuthenticated && broadcaster.Session.User != null)
            {
                string? broadcasterId = gateway.Broadcaster.Session.User?.Id;

                if (broadcasterId != null)
                {
                    var toBroadcasterId = await broadcaster.Api.Users.GetUserIdByLogin(Username);

                    if (toBroadcasterId != null)
                    {
                        await broadcaster.Api.Chat.SendShoutout(broadcasterId, toBroadcasterId, broadcasterId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Twitch send shoutout error: {ex.Message}");
            throw;
        }
    }
}