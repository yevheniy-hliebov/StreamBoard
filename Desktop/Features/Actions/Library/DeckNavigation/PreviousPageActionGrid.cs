using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Decks.Services;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.DeckNavigation
{
    [ActionDiscriminator("deck_navigation_grid_prev_page")]
    public class PreviousPageActionGrid : DeckNavigationBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Previous Page (Grid)",
            DialogTitle: "Switch to previous grid page",
            Icon: FluentIconType.Back
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        [JsonIgnore]
        public override string Label => Metadata.Name;

        public override Task ExecuteAsync(object? data = null)
        {
            GridDeckNavigationBus.RequestPreviousPage();
            return Task.CompletedTask;
        }

        public override BaseAction Copy() => new PreviousPageActionGrid { Id = this.Id };
    }
}