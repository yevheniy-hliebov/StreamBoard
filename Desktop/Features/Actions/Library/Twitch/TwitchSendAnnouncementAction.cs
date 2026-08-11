using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Components.Enums;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_send_announcement")]
    public class TwitchSendAnnouncementAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Send Announcement",
            DialogTitle: "Send Announcement Settings",
            Icon: FluentIconType.Megaphone
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

        private string _announcement = string.Empty;

        [InputField("Announcement", Hint = "Enter message...")]
        [JsonPropertyName("message")]
        public string Announcement
        {
            get => _announcement;
            set
            {
                _announcement = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        private string _color = "primary";

        [DropdownField("Color", typeof(AnnouncementColorsOptionsProvider), Hint = "Select announcement color...")]
        [JsonPropertyName("record_state")]
        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private bool _useBot = false;

        [InputField("send announcement via bot")]
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
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Announcement))
                {
                    return Metadata.Name;
                }

                string isBot = UseBot ? ", via Bot" : "";
                return $"{Metadata.Name} ({Username}, {Announcement}{isBot})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(Announcement) || string.IsNullOrWhiteSpace(Username))
                return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var moderator = UseBot ? gateway.Bot : gateway.Broadcaster;

                if (moderator.IsAuth && moderator.User != null)
                {
                    string? broadcasterId = null;
                    string? broadcasterLogin = gateway.Broadcaster.User?.Login;
                    string moderatorId = moderator.User.Id;

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
                        Enum.TryParse<TwitchAnnouncementColor>(_color, true, out var colorEnum);
                        await moderator.Api.Chat.SendAnnouncement(broadcasterId, moderatorId, Announcement, colorEnum);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch send announcement error: {ex.Message}");
                throw;
            }
        }

        public override BaseAction Copy() => new TwitchSendAnnouncementAction
        {
            Id = this.Id,
            Username = this.Username,
            Announcement = this.Announcement,
            Color = this.Color,
            UseBot = this.UseBot,
        };
    }
}