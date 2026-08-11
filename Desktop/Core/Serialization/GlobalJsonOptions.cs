using StreamTabula.Features.Actions.Serialization;
using System.Text.Json;

namespace StreamTabula.Core.Serialization;

public class GlobalJsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = ActionSerializationContext.GetResolver()
    };
}
