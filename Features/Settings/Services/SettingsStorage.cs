using StreamBoard.Core.Services;
using StreamBoard.Features.Settings.Models;

namespace StreamBoard.Features.Settings.Services
{
    public class SettingsStorage : JsonFileStorage<SettingsModel>
    {
        public SettingsStorage() : base("settings.json") { }
    }
}
