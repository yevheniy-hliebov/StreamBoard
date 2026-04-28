using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Decks.Services;
using System.Text.Json.Serialization;

namespace StreamBoard.Features.Actions.Library.DeckNavigation
{
    [ActionDiscriminator("deck_navigation_grid_next_page")]
    public class NextPageActionGrid : DeckNavigationBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Next Page (Grid)",
            DialogTitle: "Switch to next grid page",
            Icon: FluentIconType.Forward
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        [JsonIgnore]
        public override string Label => Metadata.Name;

        public override Task ExecuteAsync(object? data = null)
        {
            GridDeckNavigationBus.RequestNextPage();
            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new NextPageActionGrid { Id = this.Id };
    }
}