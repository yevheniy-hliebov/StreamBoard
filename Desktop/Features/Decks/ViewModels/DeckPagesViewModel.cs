using GongSolutions.Wpf.DragDrop;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public partial class DeckPagesViewModel : ObservableObject, IDropTarget
    {
        private readonly IDeckPageService _pageService;

        public ObservableCollection<DeckPage> AllPages => _pageService.AllPages;

        public ICommand AddPageCommand { get; }
        public ICommand DuplicatePageCommand { get; }
        public ICommand RenamePageCommand { get; }
        public ICommand EndRenameCommand { get; }
        public ICommand DeletePageCommand { get; }

        private bool _isRenameMode;
        public bool IsRenameMode
        {
            get => _isRenameMode;
            set => SetProperty(ref _isRenameMode, value);
        }

        public DeckPage? SelectedPage
        {
            get => AllPages.FirstOrDefault(p => p.Id == _pageService.GetSelectedPageId());
            set
            {
                IsRenameMode = false;
                if (value != null && _pageService.GetSelectedPageId() != value.Id)
                {
                    _pageService.SelectPage(value.Id);
                }
            }
        }

        public event Action<string, string>? PageRenamed;

        public DeckPagesViewModel(DeckStorage storage, IDeckPageService pageService)
        {
            _pageService = pageService;

            _pageService.SelectedPageChanged += () =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    OnPropertyChanged(nameof(SelectedPage));
                });
            };

            AddPageCommand = new RelayCommand(_ => _pageService.AddPage());

            DuplicatePageCommand = new RelayCommand(_ =>
            {
                if (SelectedPage != null) _pageService.DuplicatePage(SelectedPage.Id);
            }, _ => SelectedPage != null);

            DeletePageCommand = new RelayCommand(_ =>
            {
                if (SelectedPage != null) _pageService.DeletePage(SelectedPage.Id);
            }, _ => AllPages.Count > 1);

            RenamePageCommand = new RelayCommand(_ =>
            {
                if (SelectedPage != null) IsRenameMode = true;
            }, _ => SelectedPage != null);

            EndRenameCommand = new RelayCommand(_ => OnEndRename());

            var deckType = storage.Current.CanvasConfig.Type;
            if (deckType == DeckType.Grid)
            {
                GridDeckNavigationBus.Register(OnNextPage, OnPreviousPage, OnSwitchPage);
            }
        }

        public void OnEndRename()
        {
            IsRenameMode = false;

            if (SelectedPage != null)
            {
                _pageService.RenamePage(SelectedPage.Id, SelectedPage.Name);
                PageRenamed?.Invoke(SelectedPage.Id, SelectedPage.Name);
            }
        }

        // --- Drag & Drop Implementation ---
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DeckPage && !IsRenameMode)
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DeckPage)
            {
                _pageService.MovePage(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
            }
        }

        // --- DeckNavigationBus ---
        private void OnNextPage()
        {
            Application.Current.Dispatcher.Invoke(() => _pageService.NextPage());
        }

        private void OnPreviousPage()
        {
            Application.Current.Dispatcher.Invoke(() => _pageService.PreviousPage());
        }

        private void OnSwitchPage(string pageId)
        {
            Application.Current.Dispatcher.Invoke(() => _pageService.SelectPage(pageId));
        }
    }
}