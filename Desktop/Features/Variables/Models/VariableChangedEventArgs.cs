namespace StreamTabula.Features.Variables.Models;

public class VariableChangedEventArgs(string name, string value) : EventArgs
{
    public string Name { get; } = name;
    public string Value { get; } = value;
}
