using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Decks.ViewModels;

namespace StreamBoard.Features.GridDeck.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        private readonly ActionRegistry _registry;
        private readonly GridDeckStorage _storage;

        public List<ActionCategoryViewModel> ActionCategories => _registry.Categories;
        public DeckProfile Profile => _storage.CurrentProfile;

        public GridDeckViewModel(ActionRegistry registry, GridDeckStorage storage)
        {
            _registry = registry;
            _storage = storage;
        }
    }
}