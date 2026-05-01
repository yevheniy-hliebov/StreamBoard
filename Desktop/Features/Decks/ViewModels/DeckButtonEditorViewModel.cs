using StreamBoard.Core;
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

        public DeckButtonEditorViewModel(GridDeckStorage storage)
        {
            _storage = storage;

            ActionList = new ActionListViewModel(() => _storage.Save());

            ClearButtonCommand = new RelayCommand(_ =>
            {
                if (EditingSlot?.Config != null)
                {
                    EditingSlot.Config.ResetAppearance();
                    _storage.Save();
                }
            });
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