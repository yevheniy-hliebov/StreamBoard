namespace StreamBoard.Features.Decks.Services
{
    public static class GridDeckNavigationBus
    {
        public static event Action? NextPageRequested;
        public static event Action? PreviousPageRequested;
        public static event Action<string>? SwitchPageRequested;

        public static void RequestNextPage() => NextPageRequested?.Invoke();
        public static void RequestPreviousPage() => PreviousPageRequested?.Invoke();
        public static void RequestSwitchPage(string pageId) => SwitchPageRequested?.Invoke(pageId);
    }
}