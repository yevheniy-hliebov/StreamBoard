using StreamBoard.Features.Decks.Models;
using StreamBoard.Helpers;
using System.IO;

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


    public class DeckStorage<TCanvasConfig> where TCanvasConfig : new()
    {
        private readonly string _filePath;

        public DeckProfile<TCanvasConfig> CurrentProfile { get; private set; } = new();

        public DeckStorage(String fileName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _filePath = Path.Combine(baseDirectory, "data", fileName);
        }

        public void Initialize()
        {
            if (File.Exists(_filePath))
            {
                CurrentProfile = JsonHelper.Load<DeckProfile<TCanvasConfig>>(_filePath);
            }
            else
            {
                CurrentProfile = CreateDefaultProfile();
                Save();
            }
        }

        public void Save() => JsonHelper.Save(_filePath, CurrentProfile);

        private DeckProfile<TCanvasConfig> CreateDefaultProfile()
        {
            var profile = new DeckProfile<TCanvasConfig>();

            var mainPage = new DeckPageInfo { Name = "Default Page" };

            profile.Pages.List.Add(mainPage);
            profile.Pages.SelectedPageId = mainPage.Id;

            return profile;
        }
    }
}
