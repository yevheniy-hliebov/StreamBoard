using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models;

public class DynamicImageModel
{
    [JsonPropertyName("default_image")]
    public string DefaultImage { get; set; } = string.Empty;
    [JsonPropertyName("trigger_variable")]
    public string TriggerVariable { get; set; } = string.Empty;
    [JsonPropertyName("trigger_condition")]
    public TriggerConditionVariable TriggerCondition { get; set; } = TriggerConditionVariable.equal;

    [JsonPropertyName("conditions")]
    public List<DynamicImageCondition> Conditions { get; set; } = [];
}
