using StreamTabula.Core.Services;
using StreamTabula.Features.Decks.Models;

namespace StreamTabula.Features.Decks.Services
{
    public abstract class DeckStorage : JsonFileStorage<DeckProfile>
    {
        protected DeckStorage(string fileName) : base(fileName)
        {
            if (Current.PagesState.AllPages.Count == 0)
            {
                InitializeDefaultProfile();
            }
        }

        protected abstract BaseCanvasConfig CreateDefaultCanvasConfig();

        private void InitializeDefaultProfile()
        {
            Current.CanvasConfig = CreateDefaultCanvasConfig();

            var mainPage = new DeckPage { Name = "Default Page" };
            Current.PagesState.AllPages.Add(mainPage);
            Current.PagesState.SelectedPageId = mainPage.Id;

            Current.ButtonMaps[mainPage.Id] = new Dictionary<string, DeckButtonConfig>();

            Save();
        }
    }

    public class GridDeckStorage : DeckStorage
    {
        public GridDeckStorage() : base("grid_deck.json") { }

        protected override BaseCanvasConfig CreateDefaultCanvasConfig()
            => new GridCanvasConfig();
    }

    public class KeyboardDeckStorage : DeckStorage
    {
        public KeyboardDeckStorage() : base("keyboard_deck.json") { }

        protected override BaseCanvasConfig CreateDefaultCanvasConfig()
            => new KeyboardCanvasConfig();
    }
}