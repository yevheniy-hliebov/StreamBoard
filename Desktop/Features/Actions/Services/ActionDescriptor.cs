using StreamTabula.Features.Actions.Models;

namespace StreamTabula.Features.Actions.Services;

public class ActionDescriptor(string category, ActionMetadata metadata, Type actionType)
{
    public string Category { get; } = category;
    public ActionMetadata Metadata { get; } = metadata;

    public BaseAction CreateInstance() => (BaseAction)Activator.CreateInstance(actionType)!;
}