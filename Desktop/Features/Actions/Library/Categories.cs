using StreamBoard.Core.Models;
using StreamBoard.Features.Actions.Models;
using StreamBoard.Features.Actions.Attributes;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Actions.Library
{

    [ActionCategory("System", FluentIconType.System)]
    public abstract class SystemBaseAction : BaseAction { }

    [ActionCategory("Input", FluentIconType.Rename)]
    public abstract class InputBaseAction : BaseAction { }

    [ActionCategory("OBS Studio", IntegrationIconType.Obs)]
    public abstract class ObsBaseAction : BaseAction { }

    [ActionCategory("Twitch", IntegrationIconType.Twitch)]
    public abstract class TwitchBaseAction : BaseAction { }
}