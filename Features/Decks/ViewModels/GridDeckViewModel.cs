using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Decks.ViewModels;

namespace StreamBoard.Features.GridDeck.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;
        
        public DeckProfile Profile => _storage.CurrentProfile;

        public ActionLibraryViewModel Library { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            _storage = storage;
            Library = new ActionLibraryViewModel(registry);
        }
    }
}