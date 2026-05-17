using StreamTabula.Core;
using StreamTabula.Core.Services;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Actions.ViewModels;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using System.ComponentModel;

namespace StreamTabula.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }
        public DeckButtonEditorViewModel Editor { get; }
        public DeckCanvasViewModel Canvas { get; }

        private readonly WebsocketManager _wsManager;

        private readonly string _deckType = "grid";

        private CancellationTokenSource? _buttonDebounceCts;

        public GridDeckViewModel(
            GridDeckStorage storage,
            ActionRegistry registry,
            WebsocketManager wsManager,
            IClipboardService clipboard,
            IDialogService dialogService
        )
        {
            var _pageServise = new DeckPageService(storage, clipboard);
            var _buttonServise = new DeckButtonService(storage, clipboard);

            Pages = new DeckPagesViewModel(storage, _pageServise, dialogService);
            Library = new ActionLibraryViewModel(registry);
            Editor = new DeckButtonEditorViewModel(storage, dialogService, clipboard);
            Canvas = new DeckCanvasViewModel(_buttonServise, _pageServise, dialogService);

            _wsManager = wsManager;

            Canvas.PropertyChanged += OnCanvasPropertyChanged;
            _pageServise.SelectedPageChanged += OnSelectedPageChanged;
            Pages.PageRenamed += OnPageRenamed;
            Canvas.CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;
            Editor.ButtonAppearanceChanged += OnButtonAppearanceChanged;
            _buttonServise.ButtonsSwapped += OnButtonsSwapped;
            _buttonServise.ButtonChanged += OnButtonChanged;
        }

        private void OnCanvasPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckCanvasViewModel.SelectedButton))
            {
                Editor.EditingSlot = Canvas.SelectedButton;
            }
        }

        private void OnSelectedPageChanged()
        {
            var selectedPage = Pages.SelectedPage;
            if (selectedPage != null)
            {
                _ = _wsManager.BroadcastAsync(WebsocketMessageType.PageChanged, new
                {
                    deck_type = _deckType,
                    pageId = selectedPage.Id,
                    pageName = selectedPage.Name
                });
            }
        }

        private void OnPageRenamed(string pageId, string newName)
        {
            _ = _wsManager.BroadcastAsync(WebsocketMessageType.PageRenamed, new
            {
                deck_type = _deckType,
                pageId,
                pageName = newName
            });
        }

        private void OnCanvasConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridCanvasConfig.SelectedGrid))
            {
                var newGrid = ((GridCanvasConfig)sender!).SelectedGrid;

                _ = _wsManager.BroadcastAsync(WebsocketMessageType.GridLayoutChanged, new
                {
                    deck_type = _deckType,
                    grid_layout = newGrid,
                });
            }
        }

        private void OnButtonAppearanceChanged(DeckButtonSlot button)
        {
            _buttonDebounceCts?.Cancel();
            _buttonDebounceCts = new CancellationTokenSource();
            var token = _buttonDebounceCts.Token;

            var config = button.Config ?? new DeckButtonConfig();

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(3000, token);

                    if (!token.IsCancellationRequested)
                    {
                        OnButtonChanged(button.Index, config);
                    }
                }
                catch (OperationCanceledException) { }
            }, token);
        }

        private void OnButtonsSwapped(int indexA, int indexB)
        {
            _ = _wsManager.BroadcastAsync(WebsocketMessageType.ButtonsSwapped, new
            {
                deck_type = _deckType,
                index_a = indexA,
                index_b = indexB
            });
        }

        private void OnButtonChanged(int index, DeckButtonConfig config)
        {
            var name = config.Name;
            var bgColor = config.BackgroundColor;
            var imgPath = config.ImagePath;

            _ = _wsManager.BroadcastAsync(WebsocketMessageType.ButtonUpdated, new
            {
                deck_type = _deckType,
                index = index,
                name,
                background_color = bgColor,
                image_path = imgPath
            });
        }
    }
}