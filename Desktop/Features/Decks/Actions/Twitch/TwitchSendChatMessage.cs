using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_send_chat_message")]
    public class TwitchSendChatMessage : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Send Chat Message",
            DialogTitle: "Send Chat Message Settings",
            Icon: FluentIconType.Message
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _username = string.Empty;

        [ActionSetting("Channel Name", "Target username...", valueProvider: typeof(TwitchUsernameProvider))]
        [JsonPropertyName("username")]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _message = string.Empty;

        [ActionSetting("Chat Message", "Enter message...")]
        [JsonPropertyName("message")]
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        private bool _useBot = false;

        [ActionSetting("send message via bot", "")]
        [JsonPropertyName("use_bot")]
        public bool UseBot
        {
            get => _useBot;
            set => SetProperty(ref _useBot, value);
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Message))
                {
                    return Metadata.Name;
                }

                string isBot = UseBot ? ", via Bot" : "";
                return $"{Metadata.Name} ({Username}, {Message}{isBot})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(Message) || string.IsNullOrWhiteSpace(Username))
                return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var moderator = UseBot ? gateway.Bot : gateway.Broadcaster;

                if (moderator.IsAuth && moderator.User != null)
                {
                    string? broadcasterId = null;
                    string? broadcasterLogin = gateway.Broadcaster.User?.Login;
                    string senderId = moderator.User.Id;

                    if (Username == broadcasterLogin)
                    {
                        broadcasterId = gateway.Broadcaster.User?.Id;
                    }
                    else
                    {
                        if (moderator.Api != null)
                        {
                            broadcasterId = await moderator.Api.Users.GetUserIdByLogin(Username);
                        }
                    }

                    if (broadcasterId != null && moderator.Api != null)
                    {
                        await moderator.Api.Chat.SendMessage(broadcasterId, senderId, Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch send message error: {ex.Message}");
                throw;
            }
        }

        public override DeckAction Copy() => new TwitchSendChatMessage
        {
            Id = this.Id,
            Username = this.Username,
            Message = this.Message,
            UseBot = this.UseBot,
        };

    }
}