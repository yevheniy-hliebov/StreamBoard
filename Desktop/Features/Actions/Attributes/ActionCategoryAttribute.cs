using StreamTabula.Core.Models;
using StreamTabula.Features.Integrations.Common.Models;

namespace StreamTabula.Features.Actions.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ActionCategoryAttribute : Attribute
    {
        public string Name { get; }
        public FluentIconType FluentIcon { get; } = FluentIconType.Folder;
        public IntegrationIconType? IntegrationIcon { get; } = null;

        public ActionCategoryAttribute(string name)
        {
            Name = name;
            IntegrationIcon = null;
        }

        public ActionCategoryAttribute(string name, FluentIconType icon)
        {
            Name = name;
            FluentIcon = icon;
            IntegrationIcon = null;
        }

        public ActionCategoryAttribute(string name, IntegrationIconType integrationIcon)
        {
            Name = name;
            IntegrationIcon = integrationIcon;
        }
    }
}