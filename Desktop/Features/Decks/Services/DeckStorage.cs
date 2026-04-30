using StreamBoard.Core.Services;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Services
{
    public class GridDeckStorage : DeckStorage<GridCanvasConfig>
    {
        public GridDeckStorage() : base("grid_deck.json") { }
    }
    
    public class KeyboardDeckStorage : DeckStorage<GridCanvasConfig>
    {
        public KeyboardDeckStorage() : base("keyboard_deck.json") { }
    }


    public class DeckStorage<TCanvasConfig> : JsonFileStorage<DeckProfile<TCanvasConfig>>
        where TCanvasConfig : new()
    {
        public DeckStorage(string fileName) : base(fileName)
        {
            if (Current.PagesState.AllPages.Count == 0)
            {
                InitializeDefaultProfile();
            }
        }

        private void InitializeDefaultProfile()
        {
            var mainPage = new DeckPage { Name = "Default Page" };

            Current.PagesState.AllPages.Add(mainPage);
            Current.PagesState.SelectedPageId = mainPage.Id;
            var map = new Dictionary<string, DeckButtonConfig>();
            Current.ButtonMaps[mainPage.Id] = map;

            Save();
        }
    }
}
