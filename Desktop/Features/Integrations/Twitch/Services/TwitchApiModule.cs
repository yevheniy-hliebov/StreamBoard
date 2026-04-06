using System.Net.Http;
using System.Net.Http.Json;
using StreamBoard.Features.Integrations.Twitch.Exceptions;
using StreamBoard.Features.Integrations.Twitch.Models;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public abstract class TwitchApiModule(TwitchAuthContext context, HttpClient http)
    {
        protected TwitchAuthContext _context { get; } = context;
        protected HttpClient _http { get; } = http;

        protected string _baseApiUrl { get; } = "https://api.twitch.tv/helix";

        protected async Task<HttpResponseMessage> SendRequestInternal(
            HttpMethod method,
            string endpoint,
            string? query = null,
            object? body = null
        )
        {
            var uriBuilder = new UriBuilder($"{_baseApiUrl}{endpoint}");
            if (query != null)
            {
                uriBuilder.Query = query;
            }

            var request = new HttpRequestMessage(method, uriBuilder.Uri);

            request.Headers.Add("Authorization", $"Bearer {_context.AccessToken}");
            request.Headers.Add("Client-Id", _context.AppClientId);

            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var statusCode = (int)response.StatusCode;

                throw statusCode switch
                {
                    400 => new TwitchBadRequestException($"Bad Request: {errorContent}"),
                    401 => new TwitchUnauthorizedException("Unauthorized: Access token is invalid or expired."),
                    500 => new TwitchInternalServerErrorException($"Twitch API Error: {errorContent}"),
                    _ => new TwitchApiException($"Twitch API Error: {errorContent}", statusCode)
                };
            }

            return response;
        }
    }
}