namespace StreamBoard.Features.Integrations.Twitch.Models
{
    public class TwitchAuthContext(string accessToken, string tokenType, string appClientId)
    {
        public string AccessToken { get; } = accessToken;
        public string TokenType { get; } = tokenType;
        public string AppClientId { get; } = appClientId;
    }
}