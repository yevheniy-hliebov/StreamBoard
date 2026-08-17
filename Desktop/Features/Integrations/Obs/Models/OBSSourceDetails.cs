namespace StreamTabula.Features.Integrations.Obs.Models;

public record OBSSourceDetails
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public OBSSourceType Type { get; init; }

    public required string ParentName { get; init; }
    public required string SceneName { get; init; }

    public bool IsPartOfGroup => Type != OBSSourceType.Group && ParentName != SceneName;

    public bool IsNormalSource => Type == OBSSourceType.NormalSource;
    public bool IsGroup => Type == OBSSourceType.Group;
    public bool IsNestedScene => Type == OBSSourceType.NestedScene;
}
