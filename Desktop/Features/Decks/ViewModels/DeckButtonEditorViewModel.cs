using StreamBoard.Core;
using StreamBoard.Core.Services;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Actions.ViewModels;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class DeckButtonEditorViewModel : ObservableObject, IDisposable
    {
        private readonly GridDeckStorage _storage;
        private readonly IDialogService _dialogService;

        public ActionListViewModel ActionList { get; }

        private DeckButtonSlot? _editingSlot;
        public DeckButtonSlot? EditingSlot
        {
            get => _editingSlot;
            set
            {
                var oldSlot = _editingSlot;

                if (SetProperty(ref _editingSlot, value))
                {
                    ActionList.Actions = _editingSlot?.Config?.Actions;

                    if (oldSlot?.Config != null)
                    {
                        oldSlot.Config.PropertyChanged -= OnConfigPropertyChanged;
                    }

                    if (_editingSlot?.Config != null)
                    {
                        _editingSlot.Config.PropertyChanged += OnConfigPropertyChanged;
                    }
                }
            }
        }

        public ICommand ClearButtonCommand { get; }

        public event Action<DeckButtonSlot>? ButtonAppearanceChanged;

        public DeckButtonEditorViewModel(GridDeckStorage storage, IDialogService dialogService, IClipboardService clipboard)
        {
            _storage = storage;
            _dialogService = dialogService;
            var actionService = new ActionCollectionService(dialogService, clipboard);

            ActionList = new ActionListViewModel(
                onSaveRequested: () => _storage.Save(),
                actionService: actionService
            );

            ClearButtonCommand = new RelayCommand(async _ => await OnClearButtonAppearance());
        }

        private async Task OnClearButtonAppearance()
        {
            if (EditingSlot?.Config != null)
            {
                bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                    "Clear button appearance",
                    "Are you sure you want to clear the appearance of this button?");

                if (!isConfirmed) return;

                EditingSlot.Config.ResetAppearance();
                _storage.Save();
            }
        }

        private void OnConfigPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(DeckButtonConfig.Name)
                or nameof(DeckButtonConfig.ImagePath)
                or nameof(DeckButtonConfig.BackgroundColor))
            {
                _storage.Save();

                if (sender is DeckButtonConfig config)
                {
                    if (EditingSlot != null)
                    {
                        ButtonAppearanceChanged?.Invoke(EditingSlot);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (EditingSlot?.Config != null)
            {
                EditingSlot.Config.PropertyChanged -= OnConfigPropertyChanged;
            }
        }
    }
}