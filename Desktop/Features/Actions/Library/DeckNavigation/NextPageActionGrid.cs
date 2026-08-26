using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Decks.Services;

namespace StreamTabula.Features.Actions.Library.DeckNavigation;

[ActionDiscriminator("deck_navigation_grid_next_page")]
[ActionInfo("Next Page (Grid)", "Switch to next grid page", FluentIconType.Forward)]
public class NextPageActionGrid : DeckNavigationBaseAction
{
    public override Task ExecuteAsync(object? data = null)
    {
        GridDeckNavigationBus.RequestNextPage();
        return Task.CompletedTask;
    }
}