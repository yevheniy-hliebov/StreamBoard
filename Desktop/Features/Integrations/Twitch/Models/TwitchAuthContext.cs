namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchAuthContext(string accessToken, string tokenType)
    {
        public string AccessToken { get; } = accessToken;
        public string TokenType { get; } = tokenType;
    }
}