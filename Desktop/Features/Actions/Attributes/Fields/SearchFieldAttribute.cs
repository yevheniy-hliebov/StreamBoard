namespace StreamBoard.Features.Actions.Attributes
{
    public class SearchFieldAttribute(string label, Type searchProvider) : ActionFieldAttribute(label)
    {
        public Type SearchProvider { get; } = searchProvider;
        public string? DisplayProperty { get; set; }
    }
}