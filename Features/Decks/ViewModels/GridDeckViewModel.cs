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
        public ICommand RenamePageCommand { get; }
        public ICommand DeletePageCommand { get; }

        public GridDeckViewModel(GridDeckStorage storage, ActionRegistry registry)
        {
            _storage = storage;
            Library = new ActionLibraryViewModel(registry);

            AddPageCommand = new RelayCommand(_ => OnAddPage());
            DeletePageCommand = new RelayCommand(_ => OnDeletePage());
            RenamePageCommand = new RelayCommand(_ => {
                if (SelectedPage != null) IsRenameMode = true;
            }, _ => SelectedPage != null);
        }

        public DeckPageInfo? SelectedPage
        {
            get => Profile.Pages.List.FirstOrDefault(p => p.Id == Profile.Pages.SelectedPageId);
            set
            {
                IsRenameMode = false;
                if (value != null && Profile.Pages.SelectedPageId != value.Id)
                {
                    Profile.Pages.SelectedPageId = value.Id;
                    OnPageSelectionChanged(value);
                    OnPropertyChanged();
                }
            }
        }

        private bool _isRenameMode;
        public bool IsRenameMode
        {
            get => _isRenameMode;
            set => SetProperty(ref _isRenameMode, value);
        }

        private void OnAddPage()
        {
            IsRenameMode = false;

            var newPage = new DeckPageInfo
            {
                Name = $"Page {Profile.Pages.List.Count + 1}"
            };

            Profile.Pages.List.Add(newPage);
            SelectedPage = newPage;

            _storage.Save();
        }

        private void OnPageSelectionChanged(DeckPageInfo selectedPage)
        {
            System.Diagnostics.Debug.WriteLine($"Selected page: {selectedPage.Name}");
            _storage.Save();
        }

        public void EndRename()
        {
            IsRenameMode = false;
            _storage.Save();
        }

        private void OnDeletePage()
        {
            IsRenameMode = false;
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