namespace StreamBoard.Features.Actions.Models
{
    public interface IOptionsProvider
    {
        List<string> GetOptions(BaseAction action);
    }
}