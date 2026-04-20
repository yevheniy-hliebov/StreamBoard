namespace StreamBoard.Features.Actions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionSettingAttribute : Attribute
    {
        public string Label { get; }
        public string? Hint { get; }
        public Type? OptionsProvider { get; }
        public Type? ValueProvider { get; }
        public Type? SearchProvider { get; }
        public string? DisplayProperty { get; }

        public ActionSettingAttribute(
            string label,
            string? hint = null,
            Type? optionsProvider = null,
            Type? valueProvider = null,
            Type? searchProvider = null,
            string? displayProperty = null
        )
        {
            Label = label;
            Hint = hint;
            OptionsProvider = optionsProvider;
            ValueProvider = valueProvider;
            SearchProvider = searchProvider;
            DisplayProperty = displayProperty;
        }
    }
}