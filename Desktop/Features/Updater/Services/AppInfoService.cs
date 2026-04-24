using System.Reflection;
using StreamBoard.Features.Updater.Models;

namespace StreamBoard.Features.Updater.Services
{
    public class AppInfoService
    {
        public AppInfoModel AppInfo { get; }

        public AppInfoService()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var appName = assembly.GetName().Name ?? "StreamBoard";

            var infoVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var currentVersion = infoVersionAttr?.InformationalVersion ?? "1.0.0";

            AppInfo = new AppInfoModel(currentVersion, appName);
        }
    }
}