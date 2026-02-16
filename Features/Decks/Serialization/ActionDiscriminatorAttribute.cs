namespace StreamBoard.Features.Decks.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ActionDiscriminatorAttribute : Attribute
    {
        public string Discriminator { get; }
        public ActionDiscriminatorAttribute(string discriminator) => Discriminator = discriminator;
    }
}
