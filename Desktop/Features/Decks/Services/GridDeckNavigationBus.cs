using System;

namespace StreamTabula.Features.Decks.Services
{
    public static class GridDeckNavigationBus
    {
        private static event Action? NextPageRequested;
        private static event Action? PreviousPageRequested;
        private static event Action<string>? SwitchPageRequested;

        public static void Register(Action onNext, Action onPrev, Action<string> onSwitch)
        {
            NextPageRequested = null;
            PreviousPageRequested = null;
            SwitchPageRequested = null;

            NextPageRequested += onNext;
            PreviousPageRequested += onPrev;
            SwitchPageRequested += onSwitch;
        }

        public static void RequestNextPage() => NextPageRequested?.Invoke();
        public static void RequestPreviousPage() => PreviousPageRequested?.Invoke();
        public static void RequestSwitchPage(string pageId) => SwitchPageRequested?.Invoke(pageId);
    }
}