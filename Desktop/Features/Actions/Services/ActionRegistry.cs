using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.ViewModels;
using StreamTabula.Features.Integrations.Common.Models;
using System.Reflection;

namespace StreamTabula.Features.Actions.Services
{
    public class ActionRegistry
    {
        public List<ActionCategoryViewModel> Categories { get; private set; } = new();

        public void RegisterActions()
        {
            var categoryMap = new Dictionary<string, ActionCategoryViewModel>();

            var actionTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseAction)));

            foreach (var type in actionTypes)
            {
                if (Activator.CreateInstance(type) is not BaseAction tempInstance) continue;

                var attribute = type.GetCustomAttribute<ActionCategoryAttribute>();

                string categoryName = attribute?.Name ?? GetRawCategoryFromNamespace(type.Namespace);
                FluentIconType icon = attribute?.FluentIcon ?? FluentIconType.Folder;
                IntegrationIconType? integrationIcon = attribute?.IntegrationIcon;

                var descriptor = new ActionDescriptor(categoryName, tempInstance.Metadata, type);

                if (!categoryMap.TryGetValue(categoryName, out var categoryVm))
                {
                    categoryVm = new ActionCategoryViewModel(categoryName, icon, integrationIcon);
                    categoryMap[categoryName] = categoryVm;
                }

                categoryVm.Actions.Add(descriptor);
            }

            foreach (var cat in categoryMap.Values)
            {
                var sortedActions = cat.Actions.OrderBy(a => a.Metadata.Name).ToList();
                cat.Actions.Clear();
                foreach (var action in sortedActions) cat.Actions.Add(action);
            }

            Categories = categoryMap.Values.OrderBy(c => c.Name).ToList();
        }

        private string GetRawCategoryFromNamespace(string? ns)
        {
            if (string.IsNullOrEmpty(ns)) return "Uncategorized";
            return ns.Split('.').Last();
        }
    }
}