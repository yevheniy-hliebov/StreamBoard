using StreamBoard.Components.Controls;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Decks.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ActionCategoryAttribute : Attribute
    {
        public string Name { get; }
        public FluentIconType FluentIcon { get; } = FluentIconType.Folder;
        public string? ImagePath { get; set; }

        public ActionCategoryAttribute(string name)
        {
            Name = name;
        }

        public ActionCategoryAttribute(string name, FluentIconType icon)
        {
            Name = name;
            FluentIcon = icon;
        }
    }
}