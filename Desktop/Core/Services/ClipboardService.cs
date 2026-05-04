using StreamBoard.Components.Controls;
using StreamBoard.Core.Models;
using StreamBoard.Helpers;
using Wpf.Ui;
using Wpf.Ui.Controls;

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

        private readonly ISnackbarService _snackbarService;

        public event Action? ClipboardChanged;

        public ClipboardService(ISnackbarService snackbarService)
        {
            _snackbarService = snackbarService;
        }

        public void Copy<T>(T item)
        {
            if (item == null) return;

            _clipboardDataJson = JsonHelper.SerializeToString(item);
            _currentDataType = typeof(T);

            ClipboardChanged?.Invoke();

            ShowCopyNotification<T>();
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

        private void ShowCopyNotification<T>()
        {
            string itemName = typeof(T).Name;

            if (itemName.Contains("PageClipboardPayload")) itemName = "Page";
            else if (itemName.Contains("DeckButtonConfig")) itemName = "Button";
            else if (itemName.Contains("Action")) itemName = "Action";
            else itemName = "Item";

            _snackbarService.Show(
                "Copied",
                $"{itemName} copied to clipboard.",
                ControlAppearance.Secondary,
                new FluentIcon { IconType = FluentIconType.Copy },
                TimeSpan.FromSeconds(2)
            );
        }
    }
}