using GongSolutions.Wpf.DragDrop;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Servers.Models;
using StreamBoard.Features.Servers.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class DeckPagesViewModel<TCanvasConfig> : ObservableObject, IDropTarget where TCanvasConfig : new()
    {
        private readonly DeckStorage<TCanvasConfig> _storage;
        private readonly DeckPagesConfig _config;

        public ObservableCollection<DeckPageInfo> List => _config.List;

        public ICommand AddPageCommand { get; }
        public ICommand RenamePageCommand { get; }
        public ICommand EndRenameCommand { get; }
        public ICommand DeletePageCommand { get; }

        private bool _isRenameMode;
        public bool IsRenameMode
        {
            get => _isRenameMode;
            set => SetProperty(ref _isRenameMode, value);
        }

        public DeckPageInfo? SelectedPage
        {
            get => List.FirstOrDefault(p => p.Id == _config.SelectedPageId);
            set
            {
                IsRenameMode = false;
                if (value != null && _config.SelectedPageId != value.Id)
                {
                    _config.SelectedPageId = value.Id;
                    OnPropertyChanged();
                    _storage.Save();
                }
            }
        }

        public DeckPagesViewModel(DeckStorage<TCanvasConfig> storage)
        {
            _storage = storage;
            _config = storage.CurrentProfile.Pages;

            AddPageCommand = new RelayCommand(_ => OnAddPage());
            DeletePageCommand = new RelayCommand(_ => OnDeletePage(), _ => List.Count > 1);
            RenamePageCommand = new RelayCommand(_ =>
            {
                if (SelectedPage != null) IsRenameMode = true;
            }, _ => SelectedPage != null);
            EndRenameCommand = new RelayCommand(_ => OnEndRename());

            if (typeof(TCanvasConfig) == typeof(GridCanvasConfig))
            {
                GridDeckNavigationBus.NextPageRequested += OnNextPage;
                GridDeckNavigationBus.PreviousPageRequested += OnPreviousPage;
                GridDeckNavigationBus.SwitchPageRequested += OnSwitchPage;
            }
        }

        private void OnAddPage()
        {
            IsRenameMode = false;
            var newPage = new DeckPageInfo { Name = $"Page {List.Count + 1}" };
            List.Add(newPage);
            SelectedPage = newPage;
            _storage.Save();
        }

        private void OnDeletePage()
        {
            if (SelectedPage == null || List.Count <= 1) return;

            var pageToDelete = SelectedPage;
            int currentIndex = List.IndexOf(pageToDelete);
            List.Remove(pageToDelete);

            int nextIndex = Math.Min(currentIndex, List.Count - 1);
            SelectedPage = List[nextIndex];
            _storage.Save();
        }

        public event Action<string, string>? PageRenamed;

        public void OnEndRename()
        {
            IsRenameMode = false;
            _storage.Save();

            if (SelectedPage != null)
            {
                PageRenamed?.Invoke(SelectedPage.Id, SelectedPage.Name);
            }
        }

        // --- Drag & Drop Implementation ---
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DeckPageInfo && !IsRenameMode)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DeckPageInfo item && dropInfo.TargetCollection is ObservableCollection<DeckPageInfo> list)
            {
                int oldIndex = list.IndexOf(item);
                int newIndex = dropInfo.InsertIndex;

                if (oldIndex < newIndex) newIndex--;
                if (oldIndex != newIndex)
                {
                    list.Move(oldIndex, newIndex);
                    _storage.Save();
                }
            }
        }

        // DeckNabigationBus
        private void OnNextPage()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (List.Count <= 1) return;

                int currentIndex = SelectedPage != null ? List.IndexOf(SelectedPage) : -1;
                int nextIndex = (currentIndex + 1) % List.Count;

                SelectedPage = List[nextIndex];
            });
        }

        private void OnPreviousPage()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (List.Count <= 1) return;

                int currentIndex = SelectedPage != null ? List.IndexOf(SelectedPage) : -1;
                int prevIndex = (currentIndex - 1 + List.Count) % List.Count;

                SelectedPage = List[prevIndex];
            });
        }

        private void OnSwitchPage(string pageId)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var targetPage = List.FirstOrDefault(p => p.Id == pageId);
                if (targetPage != null)
                {
                    SelectedPage = targetPage;
                }
            });
        }
    }
}