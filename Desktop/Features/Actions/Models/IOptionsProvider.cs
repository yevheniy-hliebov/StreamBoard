namespace StreamTabula.Features.Actions.Models
{
    public interface IOptionsProvider
    {
        List<string> GetOptions(BaseAction action);
    }
}