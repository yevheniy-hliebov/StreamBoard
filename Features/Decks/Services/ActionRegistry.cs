using System.Reflection;
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

                string categoryName = ResolveCategoryName(type);

                var descriptor = new ActionDescriptor(categoryName, tempInstance.Metadata, type);

                if (!categoryMap.TryGetValue(categoryName, out var categoryVm))
                {
                    categoryVm = new ActionCategoryViewModel(categoryName);
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

        private string ResolveCategoryName(Type type)
        {
            var attr = type.GetCustomAttribute<ActionCategoryAttribute>();
            if (attr != null)
            {
                return attr.Name;
            }

            string rawName = GetRawCategoryFromNamespace(type.Namespace);

            return rawName;
        }

        private string GetRawCategoryFromNamespace(string? ns)
        {
            if (string.IsNullOrEmpty(ns)) return "Uncategorized";
            return ns.Split('.').Last();
        }
    }
}