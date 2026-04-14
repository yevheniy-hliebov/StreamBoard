namespace StreamBoard.Features.Decks.Models
{
    public interface IValueProvider
    {
        string GetValue(DeckAction action);
    }
}