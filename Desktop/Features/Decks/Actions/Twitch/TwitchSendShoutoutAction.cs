using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_send_shoutout")]
    public class TwitchSendShoutoutAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Send Shoutout",
            DialogTitle: "Send Shoutout Settings",
            Icon: FluentIconType.People
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _username = string.Empty;

        [ActionSetting("Username", "Enter username...")]
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

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrEmpty(Username))
                {
                    return Metadata.Name;
                }
                return $"{Metadata.Name} ({Username})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(Username)) return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (broadcaster.IsAuth && broadcaster.User != null)
                {
                    string? broadcasterId = gateway.Broadcaster.User?.Id;

                    if (broadcasterId != null && broadcaster.Api != null)
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

        public override DeckAction Copy() => new TwitchSendShoutoutAction
        {
            Id = this.Id,
            Username = this.Username
        };
    }
}