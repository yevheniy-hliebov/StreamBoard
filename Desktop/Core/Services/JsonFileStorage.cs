using System.IO;
using System.Text.Json;
using StreamTabula.Core.Serialization;

namespace StreamTabula.Core.Services;

public abstract class JsonFileStorage<T> where T : class, new()
{
    private readonly string _filePath;

    private readonly Lock _fileLock = new();

    public T Current { get; private set; } = new();

    protected JsonFileStorage(string fileName) : this("data", fileName) { }

    public JsonFileStorage(string folderName, string fileName)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    
        _filePath = Path.Combine(baseDirectory, folderName, fileName);

        Load();
    }

    public void Load()
    {
        lock (_fileLock)
        {
            if (!File.Exists(_filePath))
            {
                Current = new T();
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                Current = JsonSerializer.Deserialize<T>(json, GlobalJsonOptions.Default) ?? new T();
            }
            catch (JsonException ex)
            {
                // TODO: Ideally, show the user the message "Configuration file is corrupt".
                System.Diagnostics.Debug.WriteLine($"[JsonStorage] Error parsing {_filePath}: {ex.Message}");

                BackupCorruptedFile();
                Current = new T();
            }
        }
    }

    public void Save()
    {
        lock (_fileLock)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonSerializer.Serialize(Current, GlobalJsonOptions.Default);
            File.WriteAllText(_filePath, json);
        }
    }

    private void BackupCorruptedFile()
    {
        try
        {
            string backupPath = $"{_filePath}.corrupted_{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(_filePath, backupPath, true);
        }
        catch { /* Ignore backup errors */ }
    }
}