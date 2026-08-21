namespace StreamTabula.Features.Integrations.Twitch.Models;

public class TwitchAuthOptions
{
    public TwitchAccountRole Role { get; init; }

    public string ClientId { get; init; } = string.Empty;
    public string BaseRedirectUrl { get; init; } = "http://localhost:13551/twitch";
    public bool ForceVerify { get; init; } = true;
    public List<string> Scopes { get; init; } = [];

    public string FullRedirectUri => $"{BaseRedirectUrl.TrimEnd('/')}/{Role.ToString().ToLower()}";
}
