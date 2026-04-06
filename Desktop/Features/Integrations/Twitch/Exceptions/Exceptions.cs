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

    // 500 Internal Server Error
    public class TwitchInternalServerErrorException(string message) : TwitchApiException(message, 500);
}