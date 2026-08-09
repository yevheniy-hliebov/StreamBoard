namespace StreamTabula.Core.Services.Audio;

public interface IAudioPlayer
{
    void Play(string filePath, double volumePercent = 100);
    void Stop();
}
