using StreamTabula.Core.Mvvm;
using System.Windows.Input;

namespace StreamTabula.Features.Decks.Models
{
    public class DeckEditorState : ObservableObject
    {
        private bool _isClickMode;
        public bool IsClickMode
        {
            get => _isClickMode;
            set
            {
                if (SetProperty(ref _isClickMode, value))
                {
                    OnPropertyChanged(nameof(IsEditorMode));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsEditorMode => !IsClickMode;
    }
}
