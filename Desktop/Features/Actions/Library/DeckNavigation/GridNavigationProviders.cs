using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Decks.Services;

namespace StreamBoard.Features.Actions.Library.DeckNavigation
{
    public class GridPageOptionsProvider : IOptionsProvider
    {
        public List<string> GetOptions(BaseAction action)
        {
            var storage = App.ServiceProvider.GetRequiredService<GridDeckStorage>();
            var pages = storage.CurrentProfile.Pages.List;

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
}
