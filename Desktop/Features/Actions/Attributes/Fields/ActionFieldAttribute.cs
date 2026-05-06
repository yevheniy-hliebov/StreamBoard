namespace StreamTabula.Features.Actions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ActionFieldAttribute : Attribute
    {
        public string Label { get; }
        public string? Hint { get; set; }
        public Type? DefaultValueProvider { get; set; }

        protected ActionFieldAttribute(string label)
        {
            Label = label;
        }
    }
}