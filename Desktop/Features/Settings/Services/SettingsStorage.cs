using StreamTabula.Core.Services;
using StreamTabula.Features.Settings.Models;

namespace StreamTabula.Features.Settings.Services
{
    public class SettingsStorage : JsonFileStorage<SettingsModel>
    {
        public SettingsStorage() : base("settings.json") { }
    }
}
