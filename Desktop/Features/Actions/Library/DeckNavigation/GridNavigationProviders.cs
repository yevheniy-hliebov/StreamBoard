using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Decks.Services;

namespace StreamTabula.Features.Actions.Library.DeckNavigation;

public class GridPageOptionsProvider : IOptionsProvider
{
    public IEnumerable<object> GetOptions(BaseAction action)
    {
        var storage = App.ServiceProvider.GetRequiredService<GridDeckStorage>();
        var pages = storage.Current.PagesState.AllPages;

        if (pages.Count == 0) return ["No pages found"];

        var options = pages.Select(p => $"{p.Name} [{p.Id}]").ToList();

        if (action is SwitchPageActionGrid a && string.IsNullOrEmpty(a.TargetPageId))
        {
            var first = pages.FirstOrDefault();
            if (first != null)
            {
                a.TargetPageId = first.Id;
            }
        }

        return options;
    }
}
