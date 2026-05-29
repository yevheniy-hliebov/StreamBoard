using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_send_chat_message")]
    public class TwitchSendChatMessageAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Send Chat Message",
            DialogTitle: "Send Chat Message Settings",
            Icon: FluentIconType.Message
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _username = string.Empty;

        [InputField("Channel Name", Hint = "Target username...", DefaultValueProvider = typeof(TwitchUsernameProvider))]
        [JsonPropertyName("username")]
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _message = string.Empty;

        [InputField("Chat Message", Hint = "Enter message...")]
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

        [InputField("send message via bot")]
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

        public override async Task ExecuteAsync(ActionExecutionContext context)
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

                    string resolvedUsername = ResolveVariable(Username, context);

                    if (resolvedUsername == broadcasterLogin)
                    {
                        broadcasterId = gateway.Broadcaster.User?.Id;
                    }
                    else
                    {
                        if (moderator.Api != null)
                        {
                            broadcasterId = await moderator.Api.Users.GetUserIdByLogin(resolvedUsername);
                        }
                    }

                    if (broadcasterId != null && moderator.Api != null)
                    {
                        string resolvedMessage = ResolveVariable(Message, context);
                        await moderator.Api.Chat.SendMessage(broadcasterId, senderId, resolvedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch send message error: {ex.Message}");
                throw;
            }
        }

        public override BaseAction Copy() => new TwitchSendChatMessageAction
        {
            Id = this.Id,
            Username = this.Username,
            Message = this.Message,
            UseBot = this.UseBot,
        };
    }
}