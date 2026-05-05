using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_shield_mode")]
    public class TwitchShieldModeAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Shield Mode",
            DialogTitle: "Shield Mode Settings",
            Icon: FluentIconType.DefenderApp
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _shieldModeState = "Toggle";

        [DropdownField("State", typeof(TwitchShieldModeStateOptionsProvider), Hint = "Select action...")]
        [JsonPropertyName("shield_mode_state")]
        public string ShieldModeState
        {
            get => _shieldModeState;
            set
            {
                if (SetProperty(ref _shieldModeState, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} ({ShieldModeState})";

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

                bool targetState = false;

                if (ShieldModeState == "Toggle")
                {
                    var currentStatus = await broadcaster.Api.Moderation.GetShieldModeStatus(broadcasterId, moderatorId);

                    if (currentStatus == null)
                    {
                        Debug.WriteLine("[Twitch Shield Mode] Failed to retrieve current status for Toggle.");
                        return;
                    }

                    targetState = !currentStatus.IsActive;
                }
                else
                {
                    targetState = ShieldModeState == "Enable";
                }

                await broadcaster.Api.Moderation.UpdateShieldModeStatus(broadcasterId, moderatorId, targetState);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Twitch Shield Mode] Error: {ex.Message}");
            }
        }

        public override BaseAction Copy() => new TwitchShieldModeAction
        {
            Id = this.Id,
            ShieldModeState = this.ShieldModeState
        };
    }
}