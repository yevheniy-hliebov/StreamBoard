using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.ViewModels;
using System.Reflection;

namespace StreamTabula.Features.Actions.Services;

public class ActionRegistry
{
    public List<ActionCategoryViewModel> Categories { get; private set; } = [];

    public void RegisterActions()
    {
        var categoryMap = new Dictionary<string, ActionCategoryViewModel>();

        var actionTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && t.Namespace != null
                     && t.Namespace.StartsWith("StreamTabula.Features.Actions.Library")
                     && t.IsSubclassOf(typeof(BaseAction)));

        foreach (var type in actionTypes)
        {
            var categoryAttribute = type.GetCustomAttribute<ActionCategoryAttribute>();

            string categoryName = categoryAttribute?.Name ?? GetRawCategoryFromNamespace(type.Namespace);
            FluentIconType icon = categoryAttribute?.FluentIcon ?? FluentIconType.Folder;
            IntegrationIconType? integrationIcon = categoryAttribute?.IntegrationIcon;

            var infoAttribute = type.GetCustomAttribute<ActionInfoAttribute>();
            var metadata = infoAttribute != null
                ? new ActionMetadata(infoAttribute.Name, infoAttribute.DialogTitle, infoAttribute.Icon)
                : new ActionMetadata(type.Name, "Settings", FluentIconType.Settings);

            var descriptor = new ActionDescriptor(categoryName, metadata, type);

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

    private static string GetRawCategoryFromNamespace(string? ns)
    {
        if (string.IsNullOrEmpty(ns)) return "Uncategorized";
        return ns.Split('.').Last();
    }
}