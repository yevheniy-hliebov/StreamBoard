using System.IO;
using System.Windows.Media;
using StreamTabula.Core.Services.Audio.Exceptions;

namespace StreamTabula.Core.Services.Audio;

public class AudioPlayer : IAudioPlayer
{
    private readonly MediaPlayer _player = new();

    public void Play(string filePath, double volumePercent = 100)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new AudioPathEmptyException();
        }

        Uri? audioUri = TryGetAudioUri(filePath);

        if (audioUri is null)
        {
            throw new AudioFileNotFoundException(filePath);
        }

        double volume = Math.Clamp(volumePercent, 0, 100) / 100;

        PlayAudio(audioUri, volume);
    }

    public void Stop()
    {
        _player.Stop();
    }

    private Uri? TryGetAudioUri(string filePath)
    {
        string fullPath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

        return File.Exists(fullPath) ? new Uri(fullPath) : null;
    }

    private void PlayAudio(Uri uri, double volume)
    {
        _player.Open(uri);
        _player.Volume = volume;
        _player.Play();
    }
}
