using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_send_announcement")]
[ActionInfo("Send Announcement", "Send Announcement Settings", FluentIconType.Megaphone)]
public class TwitchSendAnnouncementAction : TwitchBaseAction
{
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
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var moderator = UseBot ? gateway.Bot : gateway.Broadcaster;

            if (moderator.Session.IsAuthenticated && moderator.Session.User != null)
            {
                string? broadcasterId = null;
                string? broadcasterLogin = gateway.Broadcaster.Session.User?.Login;
                string moderatorId = moderator.Session.User.Id;

                if (Username == broadcasterLogin)
                {
                    broadcasterId = gateway.Broadcaster.Session.User?.Id;
                }
                else
                {
                    broadcasterId = await moderator.Api.Users.GetUserIdByLogin(Username);
                }

                if (broadcasterId != null)
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
}