using System.IO;
using System.Text.Json;
using StreamBoard.Features.Decks.Serialization;

namespace StreamBoard.Helpers
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = ActionSerializationContext.GetResolver()
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
                return JsonSerializer.Deserialize<T>(json, _options) ?? new T();
            }
            catch
            {
                return new T();
            }
        }
    }
}
