using StreamTabula.Features.Actions.Models;

namespace StreamTabula.Features.Actions.Services
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

        public BaseAction CreateInstance()
        {
            return (BaseAction)Activator.CreateInstance(_actionType)!;
        }
    }
}
