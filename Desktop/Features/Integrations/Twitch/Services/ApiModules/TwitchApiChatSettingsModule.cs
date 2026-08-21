using System.Net.Http;
using System.Net.Http.Json;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Models.Requests;
using StreamTabula.Features.Integrations.Twitch.Models.Responses;

namespace StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

public class TwitchApiChatSettingsModule(ITwitchSession session, HttpClient http)
    : TwitchApiModule(session, http)
{
    public async Task<TwitchUpdateChatSettingsRequest?> UpdateChatSettings(
        string broadcasterId,
        string moderatorId,
        TwitchUpdateChatSettingsRequest requestData
    )
    {
        var query = $"broadcaster_id={broadcasterId}&moderator_id={moderatorId}";

        try
        {
            var response = await SendRequestInternal(HttpMethod.Patch, "/chat/settings", query, requestData);

            var result = await response.Content.ReadFromJsonAsync<TwitchResponse<TwitchUpdateChatSettingsRequest>>();

            return result?.Data?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            if (ex is Exceptions.TwitchApiException) throw;
            throw new Exception($"Failed to update chat settings: {ex.Message}", ex);
        }
    }

    public async Task ToggleEmoteMode(string broadcasterId, string moderatorId, bool enabled)
    {
        await UpdateChatSettings(broadcasterId, moderatorId, new TwitchUpdateChatSettingsRequest
        {
            EmoteMode = enabled
        });
    }

    public async Task ToggleFollowersMode(string broadcasterId, string moderatorId, bool enabled, int durationMinutes = 0)
    {
        await UpdateChatSettings(broadcasterId, moderatorId, new TwitchUpdateChatSettingsRequest
        {
            FollowerMode = enabled,
            FollowerModeDuration = enabled ? durationMinutes : null
        });
    }

    public async Task ToggleSubscribersMode(string broadcasterId, string moderatorId, bool enabled)
    {
        await UpdateChatSettings(broadcasterId, moderatorId, new TwitchUpdateChatSettingsRequest
        {
            SubscriberMode = enabled
        });
    }

    public async Task ToggleSlowMode(string broadcasterId, string moderatorId, bool enabled, int waitTimeSeconds = 30)
    {
        await UpdateChatSettings(broadcasterId, moderatorId, new TwitchUpdateChatSettingsRequest
        {
            SlowMode = enabled,
            SlowModeWaitTime = enabled ? waitTimeSeconds : null
        });
    }
}