namespace StreamTabula.Features.Updater.Models
{
    public class AppInfoModel(string version, string appName)
    {
        public string CurrentVersion { get; set; } = version;
        public string AppName { get; set; } = appName;
    }
}