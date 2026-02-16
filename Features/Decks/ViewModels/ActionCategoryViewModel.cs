using StreamBoard.Core;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class ActionCategoryViewModel(string name) : ObservableObject
    {
        public string Name { get; } = name;
        public ObservableCollection<ActionDescriptor> Actions { get; } = [];
    }
}
