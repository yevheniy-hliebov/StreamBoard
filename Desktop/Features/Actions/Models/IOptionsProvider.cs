namespace StreamTabula.Features.Actions.Models
{
    public interface IOptionsProvider
    {
        IEnumerable<object> GetOptions(BaseAction action);
    }
}