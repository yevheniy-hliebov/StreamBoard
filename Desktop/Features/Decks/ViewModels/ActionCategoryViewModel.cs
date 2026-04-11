using StreamBoard.Core;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Integrations.Common.Models;
using System.Collections.ObjectModel;

namespace StreamBoard.Features.Decks.ViewModels
{
    public class ActionCategoryViewModel : ObservableObject
    {
        public string Name { get; }
        public FluentIconType Icon { get; }
        public IntegrationIconType? IntegrationIcon { get; }

        public ObservableCollection<ActionDescriptor> Actions { get; } = [];

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public ActionCategoryViewModel(string name, FluentIconType icon, IntegrationIconType? integrationIcon = null)
        {
            Name = name;
            Icon = icon;
            IntegrationIcon = integrationIcon;
        }
    }
}
