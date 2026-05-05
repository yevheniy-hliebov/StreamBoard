using System.Reflection;
using StreamTabula.Features.Updater.Models;

namespace StreamTabula.Features.Updater.Services
{
    public class AppInfoService
    {
        public AppInfoModel AppInfo { get; }

        public AppInfoService()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var appName = assembly.GetName().Name ?? "StreamTabula";

            var infoVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var currentVersion = infoVersionAttr?.InformationalVersion ?? "1.0.0";

            AppInfo = new AppInfoModel(currentVersion, appName);
        }
    }
}