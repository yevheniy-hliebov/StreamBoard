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

            _clipboardDataJson = JsonHelper.SerializeToString(item);
            _currentDataType = typeof(T);

            ClipboardChanged?.Invoke();

            ShowNotification<T>(isCut: false);
        }

        public void Cut<T>(T item)
        {
            if (item == null) return;

            _clipboardDataJson = JsonHelper.SerializeToString(item);
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

        private void ShowNotification<T>(bool isCut)
        {
            string itemName = typeof(T).Name;

            if (itemName.Contains("PageClipboardPayload")) itemName = "Page";
            else if (itemName.Contains("DeckButtonConfig")) itemName = "Button";
            else if (itemName.Contains("Action")) itemName = "Action";
            else itemName = "Item";

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
    }
}