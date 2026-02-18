using StreamBoard.Components.Controls;
using StreamBoard.Core;
using StreamBoard.Features.Decks.Services;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class ActionCategoryViewModel : ObservableObject
    {
        public string Name { get; }
        public FluentIconType Icon { get; }
        public string? ImagePath { get; }

        public ObservableCollection<ActionDescriptor> Actions { get; } = [];

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public ActionCategoryViewModel(string name, FluentIconType icon, string? imagePath = null)
        {
            Name = name;
            Icon = icon;
            ImagePath = imagePath;
        }
    }
}
