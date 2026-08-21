using System.Web;

namespace StreamTabula.Features.Integrations.Twitch.Models;

public class TwitchAuthUriBuilder(TwitchAuthOptions options)
{
    private const string BaseAuthUrl = "https://id.twitch.tv/oauth2/authorize";

    public static string GenerateState()
    {
        return Guid.NewGuid().ToString("N");
    }

    public string Build(string? state = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            throw new InvalidOperationException("ClientId must be provided in options.");
        }

        var queryParams = new List<string>
        {
            "response_type=token",
            $"client_id={options.ClientId}",
            $"redirect_uri={HttpUtility.UrlEncode(options.FullRedirectUri)}"
        };

        if (options.ForceVerify)
        {
            queryParams.Add("force_verify=true");
        }

        if (options.Scopes.Count > 0)
        {
            string scopesString = string.Join(" ", options.Scopes);
            queryParams.Add($"scope={HttpUtility.UrlEncode(scopesString)}");
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            queryParams.Add($"state={state}");
        }

        return $"{BaseAuthUrl}?{string.Join("&", queryParams)}";
    }
}