using StreamTabula.Core.Services;
using StreamTabula.Features.Settings.Models;

namespace StreamTabula.Features.Settings.Services;

public partial class SettingsStorage : JsonFileStorage<SettingsModel>
{
    private const int LatestVersion = 2;

    public SettingsStorage() : base("settings.json") { }

    protected override bool Migrate()
    {
        if (Current.ConfigVersion >= LatestVersion) return false;

        while (Current.ConfigVersion < LatestVersion)
        {
            switch (Current.ConfigVersion)
            {
                case 1:
                    MigrateV1ToV2();
                    break;
                default:
                    Current.ConfigVersion = LatestVersion;
                    break;
            }
        }

        Current.OldProperties = null;
        return true;
    }
}
