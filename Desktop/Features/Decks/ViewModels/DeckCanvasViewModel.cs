using StreamBoard.Core;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class DeckCanvasViewModel : ObservableObject, IDisposable
    {
        private readonly IDeckButtonService _buttonService;
        private readonly IDeckPageService _pageService;

        public BaseCanvasConfig CanvasConfig { get; }
        public ObservableCollection<DeckButtonSlot> Buttons { get; } = [];

        private bool _isClickMode;
        public bool IsClickMode
        {
            get => _isClickMode;
            set
            {
                if (SetProperty(ref _isClickMode, value) && _isClickMode)
                {
                    SelectedButton = null;
                }
            }
        }

        public ICommand ClickButtonCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand DeleteCommand { get; }


        private DeckButtonSlot? _selectedButton;
        public DeckButtonSlot? SelectedButton
        {
            get => _selectedButton;
            set
            {
                if (value != null && value.Config == null && !IsClickMode)
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

        public DeckCanvasViewModel(IDeckButtonService buttonService, IDeckPageService pageService)
        {
            _buttonService = buttonService;
            _pageService = pageService;

            CanvasConfig = _buttonService.CanvasConfig;

            _pageService.SelectedPageChanged += RebuildButtons;
            CanvasConfig.PropertyChanged += OnCanvasConfigPropertyChanged;

            ClickButtonCommand = new RelayCommand(async p => await ExecuteClickButton(p));
            CopyCommand = new RelayCommand(_ => ExecuteCopy());
            PasteCommand = new RelayCommand(_ => ExecutePaste(), _ => _buttonService.CanPaste());
            DeleteCommand = new RelayCommand(_ => ExecuteDelete());

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

            if (!IsClickMode)
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

        private void ExecutePaste()
        {
            if (SelectedButton != null)
            {
                _buttonService.PasteButton(SelectedButton.Index);
                RebuildButtons();
            }
        }

        private void ExecuteDelete()
        {
            if (SelectedButton != null)
            {
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

        private void HandleDrop(object payload, DeckButtonSlot target)
        {
            if (IsClickMode) return;

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