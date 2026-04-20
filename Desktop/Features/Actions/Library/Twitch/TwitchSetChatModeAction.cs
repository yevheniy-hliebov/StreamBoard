using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_set_chat_mode")]
    public class TwitchSetChatModeAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Set Chat Mode",
            DialogTitle: "Set Chat Mode Settings",
            Icon: FluentIconType.ChatBubbles
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _chatMode = "Emote-Only";
        private bool _enableMode = true;
        private int _parameterValue = 0;

        [ActionSetting("Mode", "Select chat mode...", typeof(TwitchChatModeOptionsProvider))]
        [JsonPropertyName("chat_mode")]
        public string ChatMode
        {
            get => _chatMode;
            set
            {
                if (SetProperty(ref _chatMode, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [ActionSetting("Enable", "Enable or disable the selected mode")]
        [JsonPropertyName("enable_mode")]
        public bool EnableMode
        {
            get => _enableMode;
            set
            {
                if (SetProperty(ref _enableMode, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [ActionSetting("Duration / Wait Time", "Followers: 0-129600 mins. Slow: 3-120 secs. 0 = default.")]
        [JsonPropertyName("parameter_value")]
        public int ParameterValue
        {
            get => _parameterValue;
            set
            {
                int val = value < 0 ? 0 : value;
                SetProperty(ref _parameterValue, val);
            }
        }
        [JsonIgnore]
        public override string Label => $"{Metadata.Name} ({ChatMode}: {(EnableMode ? "On" : "Off")})";

        public override async Task ExecuteAsync(object? data = null)
        {
            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (!broadcaster.IsAuth || broadcaster.User?.Id == null || broadcaster.Api == null)
                {
                    return;
                }

                string broadcasterId = broadcaster.User.Id;
                string moderatorId = broadcasterId;

                switch (ChatMode)
                {
                    case "Emote-Only":
                        await broadcaster.Api.ChatSettings.ToggleEmoteMode(broadcasterId, moderatorId, EnableMode);
                        break;

                    case "Followers-Only":
                        await broadcaster.Api.ChatSettings.ToggleFollowersMode(broadcasterId, moderatorId, EnableMode, ParameterValue);
                        break;

                    case "Subscribers-Only":
                        await broadcaster.Api.ChatSettings.ToggleSubscribersMode(broadcasterId, moderatorId, EnableMode);
                        break;

                    case "Slow Mode":
                        int waitTime = (EnableMode && ParameterValue <= 0) ? 30 : ParameterValue;
                        await broadcaster.Api.ChatSettings.ToggleSlowMode(broadcasterId, moderatorId, EnableMode, waitTime);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch set chat mode error: {ex.Message}");
            }
        }

        public override BaseAction Copy() => new TwitchSetChatModeAction
        {
            Id = this.Id,
            ChatMode = this.ChatMode,
            EnableMode = this.EnableMode,
            ParameterValue = this.ParameterValue
        };
    }
}