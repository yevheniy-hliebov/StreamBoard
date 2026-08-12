using GongSolutions.Wpf.DragDrop;
using StreamTabula.Core.Mvvm;
using StreamTabula.Core.Services;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Decks.ViewModels;

public partial class DeckPagesViewModel : ObservableObject, IDropTarget, IDisposable
{
    private readonly IDeckPageService _pageService;
    private readonly IDialogService _dialogService;
    private readonly DeckEditorState _editorState;

    public ObservableCollection<DeckPage> AllPages => _pageService.AllPages;

    public IRelayCommand<object?> AddPageCommand { get; }
    public IRelayCommand<object?> CopyPageCommand { get; }
    public IRelayCommand<object?> PastePageCommand { get; }
    public IRelayCommand<object?> CutPageCommand { get; }
    public IRelayCommand<object?> DuplicatePageCommand { get; }
    public IRelayCommand<object?> RenamePageCommand { get; }
    public IRelayCommand<object?> EndRenameCommand { get; }
    public IRelayCommand<object?> DeletePageCommand { get; }

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

    public DeckPagesViewModel(
        DeckStorage storage,
        IDeckPageService pageService,
        IDialogService dialogService,
        DeckEditorState editorState)
    {
        _pageService = pageService;
        _dialogService = dialogService;
        _editorState = editorState;

        _pageService.SelectedPageChanged += OnSelectedPageChanged;
        _editorState.PropertyChanged += OnEditorStatePropertyChanged;
        AllPages.CollectionChanged += OnPagesCollectionChanged;

        AddPageCommand = new RelayCommand<object?>(
            _ => _pageService.AddPage(),
            _ => _editorState.IsEditorMode);

        CopyPageCommand = new RelayCommand<object?>(
            _ => { if (SelectedPage != null) _pageService.CopyPage(SelectedPage.Id); },
            _ => SelectedPage != null && _editorState.IsEditorMode);

        PastePageCommand = new RelayCommand<object?>(
            _ => _pageService.PastePage(),
            _ => _editorState.IsEditorMode);

        DuplicatePageCommand = new RelayCommand<object?>(
            _ => { if (SelectedPage != null) _pageService.DuplicatePage(SelectedPage.Id); },
            _ => SelectedPage != null && _editorState.IsEditorMode);

        CutPageCommand = new RelayCommand<object?>(
            async _ => await OnCutPage(),
            _ => AllPages.Count > 1 && _editorState.IsEditorMode);

        DeletePageCommand = new RelayCommand<object?>(
            async _ => await OnDeletePage(),
            _ => AllPages.Count > 1 && _editorState.IsEditorMode);

        RenamePageCommand = new RelayCommand<object?>(
            _ => { if (SelectedPage != null) IsRenameMode = true; },
            _ => SelectedPage != null && _editorState.IsEditorMode);

        EndRenameCommand = new RelayCommand<object?>(_ => OnEndRename());

        var deckType = storage.Current.CanvasConfig.Type;
        if (deckType == DeckType.Grid)
        {
            GridDeckNavigationBus.Register(OnNextPage, OnPreviousPage, OnSwitchPage);
        }
    }

    private void OnSelectedPageChanged()
    {
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            OnPropertyChanged(nameof(SelectedPage));
            UpdateCommandStates();
        });
    }

    private void OnEditorStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DeckEditorState.IsEditorMode))
        {
            UpdateCommandStates();
        }
    }

    private void OnPagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateCommandStates();
    }

    private void UpdateCommandStates()
    {
        AddPageCommand.NotifyCanExecuteChanged();
        CopyPageCommand.NotifyCanExecuteChanged();
        PastePageCommand.NotifyCanExecuteChanged();
        CutPageCommand.NotifyCanExecuteChanged();
        DuplicatePageCommand.NotifyCanExecuteChanged();
        RenamePageCommand.NotifyCanExecuteChanged();
        DeletePageCommand.NotifyCanExecuteChanged();
    }

    private async Task OnDeletePage()
    {
        if (SelectedPage != null)
        {
            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                "Delete Page",
                "Are you sure you want to delete this page? All configured buttons with appearance and actions will be lost.");

            if (!isConfirmed) return;

            _pageService.DeletePage(SelectedPage.Id);
        }
    }

    private async Task OnCutPage()
    {
        if (SelectedPage != null)
        {
            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                "Cut Page",
                "Are you sure you want to cut this page? If you don't paste, all configured buttons with appearance and actions will be lost.");

            if (!isConfirmed) return;

            _pageService.CutPage(SelectedPage.Id);
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

    public void Dispose()
    {
        _pageService.SelectedPageChanged -= OnSelectedPageChanged;
        _editorState.PropertyChanged -= OnEditorStatePropertyChanged;
        AllPages.CollectionChanged -= OnPagesCollectionChanged;

        GC.SuppressFinalize(this);
    }
}