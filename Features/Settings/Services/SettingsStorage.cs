using StreamBoard.Features.Settings.Models;
using StreamBoard.Helpers;
using System.IO;

namespace StreamBoard.Features.Settings.Services
{
    public class SettingsStorage
    {
        public readonly string _filePath;

        public SettingsModel Current { get; private set; } = new();

        public SettingsStorage()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _filePath = Path.Combine(baseDirectory, "data", "settings.json");

            Load();
        }

        public void Load()
        {
            Current = JsonHelper.Load<SettingsModel>(_filePath);
        }

        public void Save()
        {
            JsonHelper.Save(_filePath, Current);
        }
    }
}
