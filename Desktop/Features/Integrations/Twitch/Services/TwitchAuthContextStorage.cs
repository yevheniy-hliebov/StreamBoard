using StreamTabula.Features.Integrations.Twitch.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public interface ITwitchAuthContextStorage
{
    Task<TwitchAuthContext?> LoadAsync();
    Task SaveAsync(TwitchAuthContext newContext);
    void Clean();
}

public class TwitchAuthContextStorage : ITwitchAuthContextStorage
{
    private const string AdditionalEntropy = "StreamTabula_Twitch_Context";
    private readonly byte[] _entropy = Encoding.UTF8.GetBytes(AdditionalEntropy);

    private readonly string _dataDirectory;
    private readonly TwitchAccountRole _role;
    private string FilePath => Path.Combine(_dataDirectory, $"auth_{_role.ToString().ToLowerInvariant()}.bin");

    public TwitchAuthContextStorage(string dataDirectory, TwitchAccountRole role)
    {
        _dataDirectory = dataDirectory;

        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }

        _role = role;
    }

    public async Task<TwitchAuthContext?> LoadAsync()
    {
        try
        {
            byte[] loadedData = await File.ReadAllBytesAsync(FilePath);
            return DecryptAndDeserializeContext(loadedData);
        }
        catch (CryptographicException)
        {
            Clean();
            return null;
        }
        catch (JsonException)
        {
            Clean();
            return null;
        }
        catch (Exception ex) when (ex is FileNotFoundException ||
                           ex is DirectoryNotFoundException ||
                           ex is UnauthorizedAccessException)
        {
            return null;
        }
    }

    public async Task SaveAsync(TwitchAuthContext newContext)
    {
        var encryptedContext = SerializeAndEncryptContext(newContext);

        try
        {
            string tempFilePath = FilePath + ".tmp";
            await File.WriteAllBytesAsync(tempFilePath, encryptedContext);
            File.Move(tempFilePath, FilePath, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"Failed to save Twitch auth context for role {_role}.", ex);
        }
    }

    public void Clean()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }

    private byte[] SerializeAndEncryptContext(TwitchAuthContext context)
    {
        string json = JsonSerializer.Serialize(context);
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(json);

        return ProtectedData.Protect(dataToEncrypt, _entropy, DataProtectionScope.CurrentUser);
    }

    private TwitchAuthContext DecryptAndDeserializeContext(byte[] data)
    {
        byte[] decryptedData = ProtectedData.Unprotect(
                data,
                _entropy,
                DataProtectionScope.CurrentUser);

        string json = Encoding.UTF8.GetString(decryptedData);

        return JsonSerializer.Deserialize<TwitchAuthContext>(json)
        ?? throw new JsonException("Failed to deserialize Twitch authentication context.");
    }
}