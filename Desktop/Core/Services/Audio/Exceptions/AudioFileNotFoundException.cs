namespace StreamTabula.Core.Services.Audio.Exceptions;

public class AudioFileNotFoundException : Exception
{
    public AudioFileNotFoundException(string filePath)
        : base($"Audio file not found: '{filePath}'.")
    {
    }
}