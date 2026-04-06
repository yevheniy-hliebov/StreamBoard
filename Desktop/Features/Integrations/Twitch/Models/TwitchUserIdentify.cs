using System.Text.Json.Serialization;

namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchUserIdentify(
        string id,
        string login,
        string displayName,
        string type,
        string broadcasterType,
        string description,
        string profileImageUrl,
        string offlineImageUrl,
        string email, string createdAt
    )
    {
        [JsonPropertyName("id")]
        string Id { get; } = id;

        [JsonPropertyName("login")]
        string Login { get; } = login;

        [JsonPropertyName("display_name")]
        string DisplayName { get; } = displayName;

        [JsonPropertyName("type")]
        string Type { get; } = type;

        [JsonPropertyName("broadcaster_type")]
        string BroadcasterType { get; } = broadcasterType;

        [JsonPropertyName("description")]
        string Description { get; } = description;

        [JsonPropertyName("profile_image_url")]
        string ProfileImageUrl { get; } = profileImageUrl;

        [JsonPropertyName("offline_image_url")]
        string OfflineImageUrl { get; } = offlineImageUrl;

        [JsonPropertyName("email")]
        string Email { get; } = email;

        [JsonPropertyName("created_at")]
        string CreatedAt { get; } = createdAt;
    }
}