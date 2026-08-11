using StreamTabula.Components.Controls;
using StreamTabula.Components.Enums;
using StreamTabula.Core.Serialization;
using System.Text.Json;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace StreamTabula.Core.Services
{
    public interface IClipboardService
    {
        event Action? ClipboardChanged;

        void Copy<T>(T item);
        void Cut<T>(T item);
        T? Paste<T>();
        bool HasDataOfType<T>();
        void Clear();
    }

    public class ClipboardService : IClipboardService
    {
        private string? _clipboardDataJson;
        private Type? _currentDataType;

        private readonly ISnackbarService _snackbarService;

        public event Action? ClipboardChanged;

        public ClipboardService(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;
        }

        public void Copy<T>(T item)
        {
            if (item == null) return;

            _clipboardDataJson = JsonSerializer.Serialize(item, GlobalJsonOptions.Default);
            _currentDataType = typeof(T);

            ClipboardChanged?.Invoke();

            ShowNotification<T>(isCut: false);
        }

        public void Cut<T>(T item)
        {
            if (item == null) return;

            _clipboardDataJson = JsonSerializer.Serialize(item, GlobalJsonOptions.Default);
            _currentDataType = typeof(T);

            ClipboardChanged?.Invoke();

            ShowNotification<T>(isCut: true);
        }

        public T? Paste<T>()
        {
            if (!HasDataOfType<T>() || string.IsNullOrEmpty(_clipboardDataJson))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(_clipboardDataJson, GlobalJsonOptions.Default);
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

        private void ShowNotification<T>(bool isCut)
        {
            string itemName = GetFriendlyName(typeof(T));

            string actionVerb = isCut ? "cut" : "copied";
            string title = isCut ? "Cut" : "Copied";

            var icon = isCut ? FluentIconType.Cut : FluentIconType.Copy;

            _snackbarService.Show(
                title,
                $"{itemName} {actionVerb} to clipboard.",
                ControlAppearance.Secondary,
                new FluentIcon { IconType = icon },
                TimeSpan.FromSeconds(2)
            );
        }

        private static string GetFriendlyName(Type type)
        {
            string name = type.Name;

            if (name.Contains("PageClipboardPayload")) return "Page";
            if (name.Contains("DeckButtonConfig")) return "Button";
            if (name.Contains("Action")) return "Action";

            return "Item";
        }
    }
}