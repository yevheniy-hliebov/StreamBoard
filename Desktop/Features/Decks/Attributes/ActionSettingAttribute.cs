namespace StreamBoard.Features.Decks.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionSettingAttribute : Attribute
    {
        public string Label { get; }
        public string? Hint { get; }
        public Type? OptionsProvider { get; }

        public ActionSettingAttribute(string label, string? hint = null, Type? optionsProvider = null)
        {
            Label = label;
            Hint = hint;
            OptionsProvider = optionsProvider;
        }
    }
}