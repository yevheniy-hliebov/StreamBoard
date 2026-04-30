using StreamBoard.Features.Decks.Models;
using System.Text.RegularExpressions;

namespace StreamBoard.Features.Decks.Services
{
    public interface IDeckPageService
    {
        public event Action? SelectedPageChanged;
        public void AddPage();
        public void RenamePage(string id, string newName);
        public void DuplicatePage(string id);
        public void DeletePage(string id);
        public void SelectPage(string id);
        public void NextPage();
        public void PreviousPage();
        public void MovePage(int oldIndex, int newIndex);
    }

    public class DeckPageService<TCanvasConfig> : IDeckPageService where TCanvasConfig : new()
    {
        private readonly DeckStorage<TCanvasConfig> _storage;
        private readonly DeckProfile<TCanvasConfig> _profile;

        public DeckPageService(DeckStorage<TCanvasConfig> storage)
        {
            _storage = storage;
            _profile = _storage.Current;
        }

        public event Action? SelectedPageChanged;

        public void AddPage()
        {
            var pageNameRegex = new Regex(@"^Page (\d+)$");

            int maxNumber = 0;

            foreach (var page in _profile.PagesState.AllPages)
            {
                var match = pageNameRegex.Match(page.Name);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int number))
                    {
                        if (number > maxNumber) maxNumber = number;
                    }
                }
            }

            var newPage = new DeckPage
            {
                Name = $"Page {maxNumber + 1}"
            };

            var map = new Dictionary<string, DeckButtonConfig>();
            _profile.ButtonMaps[newPage.Id] = map;

            _profile.PagesState.AllPages.Add(newPage);

            _profile.PagesState.SelectedPageId = newPage.Id;

            _storage.Save();

            SelectedPageChanged?.Invoke();
        }

        public void RenamePage(string id, string newName)
        {
            var page = _profile.PagesState.AllPages.FirstOrDefault(p => p.Id == id);

            if (page != null)
            {
                page.Name = newName;
                _storage.Save();
            }
        }

        public void DuplicatePage(string id)
        {
            var originalPage = _profile.PagesState.AllPages.FirstOrDefault(p => p.Id == id);
            if (originalPage == null) return;

            var newPage = new DeckPage
            {
                Name = $"{originalPage.Name} (Copy)"
            };

            if (_profile.ButtonMaps.TryGetValue(id, out var originalButtons))
            {
                var clonedButtons = new Dictionary<string, DeckButtonConfig>(originalButtons);
                _profile.ButtonMaps[newPage.Id] = clonedButtons;
            }
            else
            {
                var map = new Dictionary<string, DeckButtonConfig>();
                _profile.ButtonMaps[newPage.Id] = map;
            }

            _profile.PagesState.AllPages.Add(newPage);

            _profile.PagesState.SelectedPageId = newPage.Id;

            _storage.Save();

            SelectedPageChanged?.Invoke();
        }

        public void DeletePage(string id)
        {
            if (_profile.PagesState.AllPages.Count <= 1) return;

            var pageToDelete = _profile.PagesState.AllPages.FirstOrDefault(p => p.Id == id);
            if (pageToDelete == null) return;

            int currentIndex = _profile.PagesState.AllPages.IndexOf(pageToDelete);

            string nextSelectedId = _profile.PagesState.SelectedPageId;
            if (_profile.PagesState.SelectedPageId == id)
            {
                int nextIndex = (currentIndex == _profile.PagesState.AllPages.Count - 1)
                    ? currentIndex - 1
                    : currentIndex + 1;

                nextSelectedId = _profile.PagesState.AllPages[nextIndex].Id;
            }

            _profile.PagesState.AllPages.Remove(pageToDelete);
            _profile.ButtonMaps.Remove(id);

            _profile.PagesState.SelectedPageId = nextSelectedId;

            _storage.Save();
            SelectedPageChanged?.Invoke();
        }

        public void SelectPage(string id)
        {
            _profile.PagesState.SelectedPageId = id;
            _storage.Save();

            SelectedPageChanged?.Invoke();
        }

        public void NextPage()
        {
            if (_profile.PagesState.AllPages.Count <= 1) return;

            var currentIndex = GetCurrentPageIndex();
            int nextIndex = (currentIndex + 1) % _profile.PagesState.AllPages.Count;

            _profile.PagesState.SelectedPageId = _profile.PagesState.AllPages[nextIndex].Id;
            _storage.Save();

            SelectedPageChanged?.Invoke();
        }

        public void PreviousPage()
        {
            if (_profile.PagesState.AllPages.Count <= 1) return;

            var currentIndex = GetCurrentPageIndex();
            int prevIndex = (currentIndex - 1 + _profile.PagesState.AllPages.Count) % _profile.PagesState.AllPages.Count;

            _profile.PagesState.SelectedPageId = _profile.PagesState.AllPages[prevIndex].Id;
            _storage.Save();

            SelectedPageChanged?.Invoke();
        }

        public void MovePage(int oldIndex, int newIndex)
        {
            var list = _profile.PagesState.AllPages;

            if (oldIndex < 0 || oldIndex >= list.Count || newIndex < 0 || newIndex > list.Count)
                return;

            int targetIndex = newIndex;
            if (oldIndex < targetIndex) targetIndex--;

            if (oldIndex != targetIndex)
            {
                list.Move(oldIndex, targetIndex);
                _storage.Save();

                SelectedPageChanged?.Invoke();
            }
        }

        private int GetCurrentPageIndex()
        {
            var currentPage = _profile.PagesState.AllPages.FirstOrDefault(p => p.Id == _profile.PagesState.SelectedPageId);
            return currentPage != null ? _profile.PagesState.AllPages.IndexOf(currentPage) : 0;
        }
    }
}
