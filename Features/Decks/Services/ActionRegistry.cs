using System.Reflection;
using Wpf.Ui.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.ViewModels;

namespace StreamBoard.Features.Decks.Services
{
    public class ActionRegistry
    {
        public List<ActionCategoryViewModel> Categories { get; private set; } = new();

        public void RegisterActions()
        {
            var categoryMap = new Dictionary<string, ActionCategoryViewModel>();

            var actionTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DeckAction)));

            foreach (var type in actionTypes)
            {
                if (Activator.CreateInstance(type) is not DeckAction tempInstance) continue;

                var attribute = type.GetCustomAttribute<ActionCategoryAttribute>();

                string categoryName = attribute?.Name ?? GetRawCategoryFromNamespace(type.Namespace);
                SymbolRegular symbol = attribute?.Symbol ?? SymbolRegular.Folder24;
                string? iconPath = attribute?.IconPath;

                var descriptor = new ActionDescriptor(categoryName, tempInstance.Metadata, type);

                if (!categoryMap.TryGetValue(categoryName, out var categoryVm))
                {
                    categoryVm = new ActionCategoryViewModel(categoryName, symbol, iconPath);
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