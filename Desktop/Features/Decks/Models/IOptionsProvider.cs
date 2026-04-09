namespace StreamBoard.Features.Decks.Models
{
    public interface IOptionsProvider
    {
        List<string> GetOptions(DeckAction action);
    }
}