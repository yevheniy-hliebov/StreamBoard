namespace StreamTabula.Features.Integrations.Twitch.Models
{
    public class TwitchAuthContext(string accessToken, string tokenType, List<string> scopes)
    {
        public string AccessToken { get; } = accessToken;
        public string TokenType { get; } = tokenType;
        public List<string> Scopes { get; } = scopes;
    }
}