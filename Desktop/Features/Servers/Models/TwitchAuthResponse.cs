using System.Text.Json.Serialization;

namespace StreamTabula.Features.Servers.Models
{
    public class TwitchAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }
}