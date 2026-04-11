using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Decks.Actions
{

    [ActionCategory("System", FluentIconType.System)]
    public abstract class SystemDeckAction : DeckAction { }

    [ActionCategory("Input", FluentIconType.Rename)]
    public abstract class InputDeckAction : DeckAction { }

    [ActionCategory("OBS Studio", IntegrationIconType.Obs)]
    public abstract class ObsDeckAction : DeckAction { }

    [ActionCategory("Twitch", IntegrationIconType.Twitch)]
    public abstract class TwitchDeckAction : DeckAction { }
}