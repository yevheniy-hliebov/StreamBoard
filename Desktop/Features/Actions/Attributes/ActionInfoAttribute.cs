using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ActionInfoAttribute(string name, string dialogTitle, FluentIconType icon) : Attribute
{
    public string Name { get; } = name;
    public string DialogTitle { get; } = dialogTitle;
    public FluentIconType Icon { get; } = icon;
}
