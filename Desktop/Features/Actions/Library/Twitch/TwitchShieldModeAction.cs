using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_shield_mode")]
[ActionInfo("Shield Mode", "Shield Mode Settings", FluentIconType.DefenderApp)]
public class TwitchShieldModeAction : TwitchBaseAction
{
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

    public override async Task ExecuteAsync(object? data = null)
    {
        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (!broadcaster.Session.IsAuthenticated || broadcaster.Session.User?.Id == null)
            {
                return;
            }

            string broadcasterId = broadcaster.Session.User.Id;
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
}