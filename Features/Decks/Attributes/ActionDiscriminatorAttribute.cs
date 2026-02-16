namespace StreamBoard.Features.Decks.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ActionDiscriminatorAttribute : Attribute
    {
        public string Discriminator { get; }
        public ActionDiscriminatorAttribute(string discriminator) => Discriminator = discriminator;
    }
}
