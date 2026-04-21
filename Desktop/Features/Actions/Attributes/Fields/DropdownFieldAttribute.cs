namespace StreamBoard.Features.Actions.Attributes
{
    public class DropdownFieldAttribute(string label, Type optionsProvider) : ActionFieldAttribute(label)
    {
        public Type OptionsProvider { get; } = optionsProvider;
    }
}