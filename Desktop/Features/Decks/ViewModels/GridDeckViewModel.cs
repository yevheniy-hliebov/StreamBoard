using StreamBoard.Core;
using StreamBoard.Features.Decks.Services;
using System.ComponentModel;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }
        public DeckButtonEditorViewModel Editor { get; }
        public DeckCanvasViewModel Canvas { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            Pages = new DeckPagesViewModel(storage);
            Library = new ActionLibraryViewModel(registry);
            Editor = new DeckButtonEditorViewModel(storage);
            Canvas = new DeckCanvasViewModel(storage);

            Canvas.PropertyChanged += OnCanvasPropertyChanged;
            Pages.PropertyChanged += OnPagesPropertyChanged;
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
            if (e.PropertyName == nameof(DeckPagesViewModel.SelectedPage))
            {
                Canvas.RebuildButtons();
            }
        }
    }
}