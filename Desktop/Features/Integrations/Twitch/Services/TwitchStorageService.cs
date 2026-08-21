using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services
{
    public class TwitchStorageService
    {
        private readonly string _dataDirectory;
        private readonly byte[] _entropy = Encoding.UTF8.GetBytes("StreamTabula_Twitch_Context_Salt");

        public TwitchStorageService()
        {
            _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public void SaveContext(TwitchAccountRole type, TwitchAuthContext context)
        {
            string json = JsonSerializer.Serialize(context);
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(json);

            byte[] encryptedData = ProtectedData.Protect(
                dataToEncrypt,
                _entropy,
                DataProtectionScope.CurrentUser);

            string filePath = GetPathForType(type);
            File.WriteAllBytes(filePath, encryptedData);
        }

        public TwitchAuthContext? LoadContext(TwitchAccountRole type)
        {
            string filePath = GetPathForType(type);
            if (!File.Exists(filePath)) return null;

            try
            {
                byte[] encryptedData = File.ReadAllBytes(filePath);

                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    _entropy,
                    DataProtectionScope.CurrentUser);

                string json = Encoding.UTF8.GetString(decryptedData);

                return JsonSerializer.Deserialize<TwitchAuthContext>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void DeleteContext(TwitchAccountRole type)
        {
            string filePath = GetPathForType(type);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetPathForType(TwitchAccountRole type)
            => Path.Combine(_dataDirectory, $"auth_{type.ToString().ToLower()}.bin");
    }
}