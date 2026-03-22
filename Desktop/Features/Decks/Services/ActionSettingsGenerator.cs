using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Decks.ViewModels;
using System.Reflection;

namespace StreamBoard.Features.Decks.Services
{
    public static class ActionSettingsGenerator
    {
        public static List<ActionSettingViewModel> GenerateSettings(DeckAction action)
        {
            var settings = new List<ActionSettingViewModel>();
            var properties = action.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<ActionSettingAttribute>();
                if (attr == null) continue;

                if (prop.PropertyType == typeof(string))
                {
                    settings.Add(new StringSettingViewModel(attr.Label, attr.Hint, action, prop));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    settings.Add(new IntSettingViewModel(attr.Label, attr.Hint, action, prop));
                }
            }

            return settings;
        }
    }
}