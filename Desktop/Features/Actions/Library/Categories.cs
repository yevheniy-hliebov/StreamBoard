using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Components.Enums;

namespace StreamTabula.Features.Actions.Library
{

    [ActionCategory("System", FluentIconType.System)]
    public abstract class SystemBaseAction : BaseAction { }

    [ActionCategory("Input", FluentIconType.Rename)]
    public abstract class InputBaseAction : BaseAction { }

    [ActionCategory("OBS Studio", IntegrationIconType.Obs)]
    public abstract class ObsBaseAction : BaseAction { }

    [ActionCategory("Twitch", IntegrationIconType.Twitch)]
    public abstract class TwitchBaseAction : BaseAction { }

    [ActionCategory("Deck Navigation", FluentIconType.Go)]
    public abstract class DeckNavigationBaseAction : BaseAction { }
}