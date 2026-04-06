namespace StreamBoard.Features.Integrations.Twitch.Exceptions
{
    public class TwitchApiException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }

    // 400 Bad Request
    public class TwitchBadRequestException(string message) : TwitchApiException(message, 400);

    // 401 Unauthorized
    public class TwitchUnauthorizedException(string message) : TwitchApiException(message, 401);

    // 403 Forbidden
    public class TwitchForbiddenException(string message) : TwitchApiException(message, 403);

    // 409 Too Many Requests / Conflict
    public class TwitchConflictException(string message) : TwitchApiException(message, 409);

    // 422 Unprocessable Entity
    public class TwitchUnprocessableEntityException(string message) : TwitchApiException(message, 422);

    // 500 Internal Server Error
    public class TwitchInternalServerErrorException(string message) : TwitchApiException(message, 500);
}