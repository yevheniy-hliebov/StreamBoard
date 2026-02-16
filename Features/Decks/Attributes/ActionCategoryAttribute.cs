namespace StreamBoard.Features.Decks.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ActionCategoryAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}