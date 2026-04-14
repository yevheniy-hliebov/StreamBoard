using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_clear_chat")]
    public class TwitchClearChatAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Clear Chat",
            DialogTitle: "Clear Chat Settings",
            Icon: FluentIconType.Delete
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _username = string.Empty;

        public override async Task ExecuteAsync(object? data = null)
        {
            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (broadcaster.IsAuth && broadcaster.User != null)
                {
                    string? broadcasterId = gateway.Broadcaster.User?.Id;

                    if (broadcasterId != null && broadcaster.Api != null)
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

        public override DeckAction Copy() => new TwitchClearChatAction { Id = this.Id };
    }
}