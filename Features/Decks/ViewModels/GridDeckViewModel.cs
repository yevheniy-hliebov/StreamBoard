using StreamBoard.Core;
using StreamBoard.Features.Decks.Services;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            Pages = new DeckPagesViewModel(storage);
            Library = new ActionLibraryViewModel(registry);
        }
    }
}