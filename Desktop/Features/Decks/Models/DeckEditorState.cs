using StreamTabula.Core;
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
                if (SetProperty(ref _isClickMode, value) && _isClickMode)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
    }
}
