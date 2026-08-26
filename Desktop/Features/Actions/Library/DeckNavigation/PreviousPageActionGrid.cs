using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Decks.Services;

namespace StreamTabula.Features.Actions.Library.DeckNavigation;

[ActionDiscriminator("deck_navigation_grid_prev_page")]
[ActionInfo("Previous Page (Grid)", "Switch to previous grid page", FluentIconType.Back)]
public class PreviousPageActionGrid : DeckNavigationBaseAction
{
    public override Task ExecuteAsync(object? data = null)
    {
        GridDeckNavigationBus.RequestPreviousPage();
        return Task.CompletedTask;
    }
}