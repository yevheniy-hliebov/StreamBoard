using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.ViewModels;
using System.Reflection;

namespace StreamTabula.Features.Actions.Services
{
    public static class ActionFieldsGenerator
    {
        public static List<ActionFieldViewModel> GenerateFields(BaseAction action)
        {
            var fields = new List<ActionFieldViewModel>();
            var properties = action.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var baseAttr = prop.GetCustomAttribute<ActionFieldAttribute>();
                if (baseAttr == null) continue;

                if (baseAttr.DefaultValueProvider != null && typeof(IValueProvider).IsAssignableFrom(baseAttr.DefaultValueProvider))
                {
                    var currentValue = prop.GetValue(action) as string;

                    if (string.IsNullOrEmpty(currentValue))
                    {
                        var valueProvider = (IValueProvider)Activator.CreateInstance(baseAttr.DefaultValueProvider)!;
                        var newValue = valueProvider.GetValue(action);

                        if (!string.IsNullOrEmpty(newValue))
                        {
                            prop.SetValue(action, newValue);
                        }
                    }
                }

                switch (baseAttr)
                {
                    case SearchFieldAttribute searchAttr when typeof(IAsyncSearchProvider).IsAssignableFrom(searchAttr.SearchProvider):
                        var searchProvider = (IAsyncSearchProvider)Activator.CreateInstance(searchAttr.SearchProvider)!;
                        fields.Add(new SearchFieldViewModel(searchAttr.Label, searchAttr.Hint, action, prop, searchProvider, searchAttr.DisplayProperty));
                        break;

                    case DropdownFieldAttribute dropdownAttr when typeof(IOptionsProvider).IsAssignableFrom(dropdownAttr.OptionsProvider):
                        var optionsProvider = (IOptionsProvider)Activator.CreateInstance(dropdownAttr.OptionsProvider)!;
                        fields.Add(new DropdownFieldViewModel(dropdownAttr.Label, dropdownAttr.Hint, action, prop, optionsProvider));
                        break;

                    case InputFieldAttribute inputAttr:
                        if (prop.PropertyType == typeof(string))
                        {
                            fields.Add(new StringFieldViewModel(inputAttr.Label, inputAttr.Hint, action, prop));
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            fields.Add(new IntFieldViewModel(inputAttr.Label, inputAttr.Hint, action, prop));
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            fields.Add(new BoolFieldViewModel(inputAttr.Label, inputAttr.Hint, action, prop));
                        }
                        break;

                    case PathFieldAttribute pathAttr:
                        fields.Add(new PathFieldViewModel(
                            pathAttr.Label,
                            pathAttr.Hint,
                            action,
                            prop,
                            pathAttr.SelectionType,
                            pathAttr.Filter
                        ));
                        break;
                }
            }

            return fields;
        }
    }
}