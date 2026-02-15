using StreamBoard.Helpers;
using StreamBoard.Features.Servers.Models;
using System.IO;

namespace StreamBoard.Features.Servers.Services
{
    public class ServerConfigsStorage
    {
        private readonly string _filePath;

        public ServerConfigs Current { get; private set; } = new();

        public ServerConfigsStorage()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(baseDir, "data", "servers.json");

            Load();
        }

        public void Load()
        {
            if (File.Exists(_filePath))
            {
                Current = JsonHelper.Load<ServerConfigs>(_filePath);
            }
        }

        public void Save() => JsonHelper.Save(_filePath, Current);
    }
}
