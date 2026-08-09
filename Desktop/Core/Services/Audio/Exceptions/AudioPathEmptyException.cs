namespace StreamTabula.Core.Services.Audio.Exceptions;

public class AudioPathEmptyException : Exception
{
    public AudioPathEmptyException(string message = "Audio path cannot be null or whitespace.")
        : base(message) { }
}
