using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Decks.Services;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.DeckNavigation
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

        public override Task ExecuteAsync(ActionExecutionContext context)
        {
            GridDeckNavigationBus.RequestNextPage();
            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new NextPageActionGrid { Id = this.Id };
    }
}