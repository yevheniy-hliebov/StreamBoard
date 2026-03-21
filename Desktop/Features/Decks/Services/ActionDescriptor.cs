using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Services
{
    public class ActionDescriptor
    {
        public string Category { get; }
        public ActionMetadata Metadata { get; }

        private readonly Type _actionType;

        public ActionDescriptor(string category, ActionMetadata metadata, Type actionType)
        {
            Category = category;
            Metadata = metadata;
            _actionType = actionType;
        }

        public DeckAction CreateInstance()
        {
            return (DeckAction)Activator.CreateInstance(_actionType)!;
        }
    }
}
