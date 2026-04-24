using System.Text.Json.Serialization;

namespace StreamBoard.Features.Updater.Models
{
    public class GithubReleaseInfo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("draft")]
        public bool Draft { get; set; } = false;

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; } = false;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("published_at")]
        public string PublishedAt { get; set; } = string.Empty;

        [JsonPropertyName("assets")]
        public List<GithubAssetInfo> Assets { get; set; } = [];

        [JsonIgnore]
        public string CleanVersion => TagName.StartsWith("v") ? TagName.Substring(1) : TagName;
    }
}