using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_clear_chat")]
[ActionInfo("Clear Chat", "Clear Chat Settings", FluentIconType.Delete)]
public class TwitchClearChatAction : TwitchBaseAction
{
    public override async Task ExecuteAsync(object? data = null)
    {
        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (broadcaster.Session.IsAuthenticated && broadcaster.Session.User != null)
            {
                string? broadcasterId = gateway.Broadcaster.Session.User?.Id;

                if (broadcasterId != null)
                {
                    await broadcaster.Api.Moderation.ClearChat(broadcasterId, broadcasterId);
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