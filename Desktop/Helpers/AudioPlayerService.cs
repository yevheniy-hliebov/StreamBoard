using System.IO;
using System.Windows.Media;

namespace StreamBoard.Helpers
{
    public static class AudioPlayerService
    {
        private static readonly MediaPlayer _player = new();

        public static void Play(string path, double volumePercent = 50)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            volumePercent = Math.Clamp(volumePercent, 0, 100);
            Uri? uri = null;

            if (Path.IsPathRooted(path) && File.Exists(path))
            {
                uri = new Uri(path);
            }
            else
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                if (File.Exists(fullPath))
                {
                    uri = new Uri(fullPath);
                }
            }

            if (uri == null) return;

            App.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    _player.Open(uri);
                    _player.Volume = volumePercent / 100.0;
                    _player.Stop();
                    _player.Play();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AudioPlayer] Помилка: {ex.Message}");
                }
            });
        }
    }
}