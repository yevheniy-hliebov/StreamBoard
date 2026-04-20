using StreamBoard.Core;
using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Integrations.Common.Models;
using System.Collections.ObjectModel;

namespace StreamBoard.Features.Actions.ViewModels
{
    public class ActionCategoryViewModel(
        string name,
        FluentIconType icon,
        IntegrationIconType? integrationIcon = null
    ) : ObservableObject
    {
        public string Name { get; } = name;
        public FluentIconType Icon { get; } = icon;
        public IntegrationIconType? IntegrationIcon { get; } = integrationIcon;

        public ObservableCollection<ActionDescriptor> Actions { get; } = [];

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
    }
}
