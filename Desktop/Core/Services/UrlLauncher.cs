using System.Diagnostics;

namespace StreamTabula.Core.Services;

public interface IUrlLauncher
{
    void OpenUrl(string url);
}

public class UrlLauncher : IUrlLauncher
{
    public void OpenUrl(string url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not open URL: {url}", ex);
        }
    }
}
