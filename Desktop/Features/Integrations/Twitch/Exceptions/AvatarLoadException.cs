namespace StreamTabula.Features.Integrations.Twitch.Exceptions;

public class AvatarLoadException : Exception
{
    public AvatarLoadException(string message, Exception innerException)
        : base(message, innerException) { }
}
