namespace StreamTabula.Features.Actions.Models
{
    public interface IValueProvider
    {
        string GetValue(BaseAction action);
    }
}