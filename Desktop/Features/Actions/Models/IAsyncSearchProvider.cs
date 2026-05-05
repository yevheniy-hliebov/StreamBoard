namespace StreamTabula.Features.Actions.Models
{
    public class SearchResult
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public override string ToString() => DisplayName;
    }

    public interface IAsyncSearchProvider
    {
        Task<IEnumerable<SearchResult>> SearchAsync(string query);
    }
}