using StreamTabula.Components.Enums;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Integrations.Common.Models;
using System.Collections.ObjectModel;

namespace StreamTabula.Features.Actions.ViewModels
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
