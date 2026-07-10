namespace StreamTabula.Components.Controls;

public class DropdownOption
{
    public object Value { get; }
    public string DisplayOption { get; }
    public string DisplaySelectedOption { get; }

    public DropdownOption(object value)
    {
        Value = value;
        DisplayOption = value?.ToString() ?? string.Empty;
        DisplaySelectedOption = DisplayOption;
    }

    public DropdownOption(object value, string displayOption)
    {
        Value = value;
        DisplayOption = displayOption;
        DisplaySelectedOption = displayOption;
    }

    public DropdownOption(object value, string displayOption, string displaySelectedOption)
    {
        Value = value;
        DisplayOption = displayOption;
        DisplaySelectedOption = displaySelectedOption;
    }
}
