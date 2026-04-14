using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StreamBoard.Features.Integrations.Twitch.Services.Auth
{
    public class TwitchAuthUriBuilder
    {
        private const string BaseAuthUrl = "https://id.twitch.tv/oauth2/authorize";

        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public bool ForceVerify { get; set; } = false;
        public List<string> Scopes { get; set; } = [];

        public static string GenerateState()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string Build(string? state = null)
        {
            if (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(RedirectUri))
            {
                throw new InvalidOperationException("ClientId and RedirectUri must be set.");
            }

            var queryParams = new List<string>
            {
                "response_type=token",
                $"client_id={ClientId}",
                $"redirect_uri={HttpUtility.UrlEncode(RedirectUri)}"
            };

            if (ForceVerify)
            {
                queryParams.Add("force_verify=true");
            }

            if (Scopes.Count > 0)
            {
                string scopesString = string.Join(" ", Scopes);
                queryParams.Add($"scope={HttpUtility.UrlEncode(scopesString)}");
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                queryParams.Add($"state={state}");
            }

            return $"{BaseAuthUrl}?{string.Join("&", queryParams)}";
        }
    }
}