using StreamBoard.Core;
using System;
using System.Collections.Generic;
using System.Text;

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

        public DeckButtonSlot(int index, DeckButtonConfig? config)
        {
            Index = index;
            _config = config;
        }
    }
}
