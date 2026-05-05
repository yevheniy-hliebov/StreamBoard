using System.Net.Http;
using System.Net.Http.Json;
using StreamTabula.Features.Integrations.Twitch.Exceptions;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services.ApiModules
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
                    404 => new TwitchNotFoundException($"Not Found: {errorContent}"),
                    403 => new TwitchForbiddenException($"Forbidden: You don't have permission for this action. {errorContent}"),
                    409 => new TwitchConflictException($"Conflict/Too Many Requests: {errorContent}"),
                    422 => new TwitchUnprocessableEntityException($"Unprocessable Entity: Message too long or failed validation. {errorContent}"),
                    500 => new TwitchInternalServerErrorException($"Twitch API Error: {errorContent}"),
                    _ => new TwitchApiException($"Twitch API Error: {errorContent}", statusCode)
                };
            }

            return response;
        }
    }
}