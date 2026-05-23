using StreamTabula.Core;
using StreamTabula.Core.Services;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Decks.Models;
using StreamTabula.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.ViewModels
{
    public class DeckCanvasViewModel : ObservableObject, IDisposable
    {
        private readonly IDeckButtonService _buttonService;
        private readonly IDeckPageService _pageService;
        private readonly IDialogService _dialogService;

        private readonly DeckEditorState _editorState;

        public BaseCanvasConfig CanvasConfig { get; }
        public ObservableCollection<DeckButtonSlot> Buttons { get; } = [];

        public ICommand ClickButtonCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand DeleteCommand { get; }


        private DeckButtonSlot? _selectedButton;
        public DeckButtonSlot? SelectedButton
        {
            get => _selectedButton;
            set
            {
                if (value != null && value.Config == null && !_editorState.IsClickMode)
                {
                    value.Config = _buttonService.GetOrCreateButton(value.Index);
                }

                if (SetProperty(ref _selectedButton, value))
                {
                    if (_selectedButton != null)
                    {
                        foreach (var button in Buttons)
                        {
                            button.IsSelected = button == _selectedButton;
                        }
                    }
                }
            }
        }

        public DeckCanvasViewModel(
            IDeckButtonService buttonService,
            IDeckPageService pageService,
            IDialogService dialogService,
            DeckEditorState editorState)
        {
            _buttonService = buttonService;
            _pageService = pageService;
            _dialogService = dialogService;

            _editorState = editorState;

            CanvasConfig = _buttonService.CanvasConfig;

            _pageService.SelectedPageChanged += RebuildButtons;
            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;
            _editorState.PropertyChanged += OnEditorStatePropertyChanged;

            ClickButtonCommand = new RelayCommand(async p => await ExecuteClickButton(p));
            CopyCommand = new RelayCommand(_ => ExecuteCopy(), _ => !_editorState.IsClickMode);
            PasteCommand = new RelayCommand(async _ => await ExecutePaste(), _ => _buttonService.CanPaste() && !_editorState.IsClickMode);
            CutCommand = new RelayCommand(async _ => await ExecuteCut(), _ => !_editorState.IsClickMode);
            DeleteCommand = new RelayCommand(async _ => await ExecuteDelete(), _ => !_editorState.IsClickMode);

            RebuildButtons();
        }

        private void RebuildButtons()
        {
            SelectedButton = null;
            Buttons.Clear();

            var map = _buttonService.GetCurrentButtonMap();

            if (CanvasConfig.Type == DeckType.Grid)
            {
                var gridCanvasConfig = (GridCanvasConfig)CanvasConfig;
                foreach (var index in gridCanvasConfig.Cells)
                {
                    map.TryGetValue(index.ToString(), out var config);
                    var slot = new DeckButtonSlot(index, config, HandleDrop);
                    Buttons.Add(slot);
                }
            }
        }

        private async Task ExecuteClickButton(object? parameter)
        {
            if (parameter is not DeckButtonSlot slot)
                return;

            if (!_editorState.IsClickMode)
            {
                if (SelectedButton == slot)
                    return;

                SelectedButton = slot;
            }
            else if (slot.Config != null)
            {
                await _buttonService.ExecuteButtonActions(slot.Config);
            }
        }

        private void ExecuteCopy()
        {
            if (SelectedButton != null)
            {
                _buttonService.CopyButton(SelectedButton.Index);

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async Task ExecuteCut()
        {
            if (SelectedButton != null)
            {
                if (SelectedButton.Config != null && SelectedButton.Config.HasData)
                {
                    bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                        "Cut Button",
                        "Are you sure you want to cut this button? If you don't paste, all actions and appearance will be lost.");

                    if (!isConfirmed) return;
                }

                _buttonService.CutButton(SelectedButton.Index);
                RebuildButtons();

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async Task ExecutePaste()
        {
            if (SelectedButton != null)
            {
                if (SelectedButton.Config != null && SelectedButton.Config.HasData)
                {
                    bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                        "Overwrite Button",
                        "This slot already has a button configured. Are you sure you want to overwrite it?");

                    if (!isConfirmed) return;
                }

                _buttonService.PasteButton(SelectedButton.Index);
                RebuildButtons();
            }
        }

        private async Task ExecuteDelete()
        {
            if (SelectedButton?.Config != null)
            {
                bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                    "Clear Button",
                    "Are you sure you want to clear this button? All actions and appearance will be lost.");

                if (!isConfirmed) return;

                _buttonService.DeleteButton(SelectedButton.Index);
                RebuildButtons();
            }
        }

        private void OnCanvasConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridCanvasConfig.SelectedGrid))
            {
                RebuildButtons();
                _buttonService.SaveChanges();
            }
        }

        private void OnEditorStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckEditorState.IsClickMode))
            {
                if (_editorState.IsClickMode)
                {
                    SelectedButton = null;
                }
            }
        }

        private void HandleDrop(object payload, DeckButtonSlot target)
        {
            if (_editorState.IsClickMode) return;

            if (payload is DeckButtonSlot sourceSlot && sourceSlot != target)
            {
                _buttonService.SwapButtons(sourceSlot.Index, target.Index);
                RebuildButtons();
            }
            else if (payload is ActionDescriptor descriptor)
            {
                _buttonService.AddActionToButton(target.Index, descriptor);
                target.Config ??= _buttonService.GetOrCreateButton(target.Index);
            }
        }

        public void Dispose()
        {
            _pageService.SelectedPageChanged -= RebuildButtons;
            CanvasConfig.PropertyChanged -= OnCanvasConfigPropertyChanged;
        }
    }
}