using StreamTabula.Features.Settings.Models;
using StreamTabula.Helpers;
using System.IO;

namespace StreamTabula.Core.Services
{
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
                Current = JsonHelper.Load<T>(_filePath);
            }
        }

        public void Save()
        {
            lock (_fileLock)
            {
                JsonHelper.Save(_filePath, Current);
            }
        }
    }
}