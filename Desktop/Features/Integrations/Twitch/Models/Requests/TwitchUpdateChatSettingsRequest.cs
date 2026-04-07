using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchUpdateChatSettingsRequest
    {
        [JsonPropertyName("emote_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? EmoteMode { get; set; }

        [JsonPropertyName("follower_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FollowerMode { get; set; }

        [JsonPropertyName("follower_mode_duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? FollowerModeDuration { get; set; }

        [JsonPropertyName("non_moderator_chat_delay")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? NonModeratorChatDelay { get; set; }

        [JsonPropertyName("non_moderator_chat_delay_duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? NonModeratorChatDelayDuration { get; set; }

        [JsonPropertyName("slow_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SlowMode { get; set; }

        [JsonPropertyName("slow_mode_wait_time")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SlowModeWaitTime { get; set; }

        [JsonPropertyName("subscriber_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SubscriberMode { get; set; }

        [JsonPropertyName("unique_chat_mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UniqueChatMode { get; set; }
    }
}