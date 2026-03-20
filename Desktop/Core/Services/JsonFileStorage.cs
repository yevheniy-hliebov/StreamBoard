using StreamBoard.Features.Settings.Models;
using StreamBoard.Helpers;
using System.IO;

namespace StreamBoard.Core.Services
{
    public abstract class JsonFileStorage<T> where T : class, new()
    {
        private readonly string _filePath;

        public T Current { get; private set; } = new();

        protected JsonFileStorage(string fileName) : this("data", fileName) { }

        public JsonFileStorage(string folderName, string fileName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _filePath = Path.Combine(baseDirectory, folderName, fileName);

            Load();
        }

        public void Load() => Current = JsonHelper.Load<T>(_filePath);

        public void Save() => JsonHelper.Save(_filePath, Current);
    }
}
