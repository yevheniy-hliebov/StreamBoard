using StreamBoard.Core;
using StreamBoard.Features.Actions.Services;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace StreamBoard.Features.Decks.Models
{
    public class DeckButtonSlot : ObservableObject
    {
        public int Index { get; }

        private DeckButtonConfig? _config;
        public DeckButtonConfig? Config
        {
            get => _config;
            set => SetProperty(ref _config, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ICommand ReceiveDropCommand { get; }

        public DeckButtonSlot(int index, DeckButtonConfig? config, Action<object, DeckButtonSlot> onDropAction)
        {
            Index = index;
            _config = config;

            ReceiveDropCommand = new RelayCommand<object>(
                execute: payload =>
                {
                    if (payload != null)
                    {
                        onDropAction(payload, this);
                    }
                },
                canExecute: payload =>
                {
                    return payload is DeckButtonSlot || payload is ActionDescriptor;
                }
            );
        }
    }
}