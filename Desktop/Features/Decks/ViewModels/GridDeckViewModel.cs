using StreamBoard.Core;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Actions.ViewModels;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Servers.Models;
using StreamBoard.Features.Servers.Services;
using System.ComponentModel;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        public DeckPagesViewModel<GridCanvasConfig> Pages { get; }
        public ActionLibraryViewModel Library { get; }
        public DeckButtonEditorViewModel Editor { get; }
        public DeckCanvasViewModel Canvas { get; }

        private readonly WebsocketManager _wsManager;

        private readonly string _deckType = "grid";

        private CancellationTokenSource? _buttonDebounceCts;

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry, WebsocketManager wsManager)
        {
            Pages = new DeckPagesViewModel<GridCanvasConfig>(storage, new DeckPageService<GridCanvasConfig>(storage));
            Library = new ActionLibraryViewModel(registry);
            Editor = new DeckButtonEditorViewModel(storage);
            Canvas = new DeckCanvasViewModel(storage);

            _wsManager = wsManager;

            Canvas.PropertyChanged += OnCanvasPropertyChanged;
            Pages.PropertyChanged += OnPagesPropertyChanged;
            Pages.PageRenamed += OnPageRenamed;
            Canvas.CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;
            Canvas.ButtonAppearanceChanged += OnButtonAppearanceChanged;
            Canvas.ButtonsSwapped += OnButtonsSwapped;
        }

        private void OnCanvasPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckCanvasViewModel.SelectedButton))
            {
                Editor.EditingSlot = Canvas.SelectedButton;
            }
        }

        private void OnPagesPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckPagesViewModel<GridCanvasConfig>.SelectedPage))
            {
                Canvas.RebuildButtons();

                var selectedPage = Pages.SelectedPage;
                if (selectedPage != null)
                {
                    _ = _wsManager.BroadcastAsync(WebsocketMessageType.PageChanged, new
                    {
                        deckType = _deckType,
                        pageId = selectedPage.Id,
                        pageName = selectedPage.Name
                    });
                }
            }
        }

        private void OnPageRenamed(string pageId, string newName)
        {
            _ = _wsManager.BroadcastAsync(WebsocketMessageType.PageRenamed, new
            {
                deckType = _deckType,
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
                    deckType = _deckType,
                    grid_layout = newGrid,
                });
            }
        }

        private void OnButtonAppearanceChanged(int index, DeckButtonConfig config)
        {
            _buttonDebounceCts?.Cancel();
            _buttonDebounceCts = new CancellationTokenSource();
            var token = _buttonDebounceCts.Token;

            var name = config.Name;
            var bgColor = config.BackgroundColor;
            var imgPath = config.ImagePath;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(3000, token);

                    if (!token.IsCancellationRequested)
                    {
                        await _wsManager.BroadcastAsync(WebsocketMessageType.ButtonAppearanceChanged, new
                        {
                            deckType = _deckType,
                            index,
                            name,
                            background_color = bgColor,
                            image_path = imgPath
                        });
                    }
                }
                catch (OperationCanceledException) { }
            }, token);
        }

        private void OnButtonsSwapped(int indexA, int indexB)
        {
            _ = _wsManager.BroadcastAsync(WebsocketMessageType.ButtonsSwapped, new
            {
                deckType = _deckType,
                index_a = indexA,
                index_b = indexB
            });
        }
    }
}