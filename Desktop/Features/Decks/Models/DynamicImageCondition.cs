using StreamTabula.Core;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Decks.Models;

public class DynamicImageCondition : ObservableObject
{
    private string _value = string.Empty;
    [JsonPropertyName("value")]
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    private string _valueFrom = string.Empty;
    [JsonPropertyName("value_from")]
    public string ValueFrom
    {
        get => _valueFrom;
        set => SetProperty(ref _valueFrom, value);
    }

    private string _valueTo = string.Empty;
    [JsonPropertyName("value_to")]
    public string ValueTo
    {
        get => _valueTo;
        set => SetProperty(ref _valueTo, value);
    }

    private string _imagePath = string.Empty;
    [JsonPropertyName("image_path")]
    public string ImagePath
    {
        get => _imagePath;
        set => SetProperty(ref _imagePath, value);
    }

    private bool _isBlinking = false;
    [JsonPropertyName("is_blinking")]
    public bool IsBlinking
    {
        get => _isBlinking;
        set => SetProperty(ref _isBlinking, value);
    }
}
