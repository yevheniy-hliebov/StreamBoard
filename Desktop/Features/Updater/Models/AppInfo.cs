namespace StreamTabula.Features.Updater.Models;

public class AppInfoModel(string version, string appName)
{
    public string CurrentVersion { get; set; } = version;
    public string AppName { get; set; } = appName;

    public string AuthorName { get; set; } = string.Empty;

    public string LinkText { get; set; } = string.Empty;
    public string LinkUrl { get; set; } = string.Empty;
}