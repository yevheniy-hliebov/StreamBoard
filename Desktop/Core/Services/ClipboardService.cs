using StreamBoard.Helpers;
using System.Text.Json;

namespace StreamBoard.Core.Services
{
    public interface IClipboardService
    {
        event Action? ClipboardChanged;

        void Copy<T>(T item);
        T? Paste<T>();
        bool HasDataOfType<T>();
        void Clear();
    }

    public class ClipboardService : IClipboardService
    {
        private string? _clipboardDataJson;
        private Type? _currentDataType;

        public event Action? ClipboardChanged;

        public void Copy<T>(T item)
        {
            if (item == null) return;

            _clipboardDataJson = JsonHelper.SerializeToString(item);
            _currentDataType = typeof(T);

            ClipboardChanged?.Invoke();
        }

        public T? Paste<T>()
        {
            if (!HasDataOfType<T>() || string.IsNullOrEmpty(_clipboardDataJson))
            {
                return default;
            }

            return JsonHelper.DeserializeFromString<T>(_clipboardDataJson);
        }

        public bool HasDataOfType<T>()
        {
            return _currentDataType == typeof(T);
        }

        public void Clear()
        {
            _clipboardDataJson = null;
            _currentDataType = null;

            ClipboardChanged?.Invoke();
        }
    }
}
