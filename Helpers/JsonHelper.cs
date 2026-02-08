using System.IO;
using System.Text.Json;

namespace StreamBoard.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
        };

        public static void Save<T>(string filePath, T data)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(filePath, json);
        }

        public static T Load<T>(String filePath) where T : new()
        {
            if (!File.Exists(filePath)) return new T();
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json) ?? new T();
            }
            catch
            {
                return new T();
            }
        }
    }
}
