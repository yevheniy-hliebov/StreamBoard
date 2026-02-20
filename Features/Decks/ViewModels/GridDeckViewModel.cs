using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.ComponentModel;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;

        public DeckPagesViewModel Pages { get; }
        public ActionLibraryViewModel Library { get; }

        public GridCanvasConfig CanvasConfig { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            _storage = storage;
            
            Pages = new DeckPagesViewModel(storage);
            Library = new ActionLibraryViewModel(registry);
            CanvasConfig = storage.CurrentProfile.CanvasConfig;

            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;
        }

        private void OnCanvasConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridCanvasConfig.SelectedGrid))
            {
                _storage.Save();
            }
        }
    }
}