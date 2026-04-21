using StreamBoard.Core;
using StreamBoard.Features.Actions.ViewModels;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.Services;
using System.Windows.Input;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class DeckButtonEditorViewModel : ObservableObject
    {
        private readonly GridDeckStorage _storage;

        public ActionListViewModel ActionList { get; }

        private DeckButtonSlot? _editingSlot;
        public DeckButtonSlot? EditingSlot
        {
            get => _editingSlot;
            set
            {
                if (SetProperty(ref _editingSlot, value))
                {
                    ActionList.Actions = _editingSlot?.Config?.Actions;
                }
            }
        }

        public ICommand ClearButtonCommand { get; }

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
    }
}