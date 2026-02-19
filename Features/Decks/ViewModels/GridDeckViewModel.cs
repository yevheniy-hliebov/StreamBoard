using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class GridDeckViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;
        
        public DeckProfile Profile => _storage.CurrentProfile;

        public ActionLibraryViewModel Library { get; }

        public ICommand AddPageCommand { get; }
        public ICommand DeletePageCommand { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            _storage = storage;
            Library = new ActionLibraryViewModel(registry);

            AddPageCommand = new RelayCommand(_ => OnAddPage());
            DeletePageCommand = new RelayCommand(_ => OnDeletePage());
        }

        public DeckPageInfo? SelectedPage
        {
            get => Profile.Pages.List.FirstOrDefault(p => p.Id == Profile.Pages.SelectedPageId);
            set
            {
                if (value != null && Profile.Pages.SelectedPageId != value.Id)
                {
                    Profile.Pages.SelectedPageId = value.Id;
                    OnPageSelectionChanged(value);
                    OnPropertyChanged();
                }
            }
        }

        private void OnAddPage()
        {
            var newPage = new DeckPageInfo
            {
                Name = $"Page {Profile.Pages.List.Count + 1}"
            };

            Profile.Pages.List.Add(newPage);
            Profile.Pages.SelectedPageId = newPage.Id;

            _storage.Save();
        }

        private void OnPageSelectionChanged(DeckPageInfo selectedPage)
        {
            System.Diagnostics.Debug.WriteLine($"Selected page: {selectedPage.Name}");
            _storage.Save();
        }

        private void OnDeletePage()
        {
            if (SelectedPage == null || Profile.Pages.List.Count == 1) return;

            var pageToDelete = SelectedPage;
            var list = Profile.Pages.List;

            int currentIndex = list.IndexOf(pageToDelete);

            list.Remove(pageToDelete);

            if (list.Count > 0)
            {
                int nextIndex = Math.Min(currentIndex, list.Count - 1);
                SelectedPage = list[nextIndex];
            }
            else
            {
                SelectedPage = null;
            }

            _storage.Save();
        }
    }
}