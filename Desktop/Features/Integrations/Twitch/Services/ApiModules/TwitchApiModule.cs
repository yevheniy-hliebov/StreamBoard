using StreamTabula.Features.Integrations.Twitch.Exceptions;
using StreamTabula.Features.Integrations.Twitch.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace StreamTabula.Features.Integrations.Twitch.Services.ApiModules;

public abstract class TwitchApiModule(ITwitchSession session, HttpClient http, string clientId)
{
    protected string BaseApiUrl { get; } = "https://api.twitch.tv/helix";

    protected async Task<HttpResponseMessage> SendRequestInternal(
        HttpMethod method,
        string endpoint,
        string? query = null,
        object? body = null,
        TwitchAuthContext? overrideContext = null
    )
    {
        var authContext = overrideContext ?? session.AuthContext
        ?? throw new TwitchUnauthorizedException("Cannot execute request: No active Twitch session.");

        var uriBuilder = new UriBuilder($"{BaseApiUrl}{endpoint}");
        if (query != null)
        {
            uriBuilder.Query = query;
        }

        var request = new HttpRequestMessage(method, uriBuilder.Uri);

        request.Headers.Add("Authorization", $"Bearer {authContext.AccessToken}");
        request.Headers.Add("Client-Id", clientId);

        if (body != null)
        {
            request.Content = JsonContent.Create(body);
        }

        var response = await http.SendAsync(request);

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