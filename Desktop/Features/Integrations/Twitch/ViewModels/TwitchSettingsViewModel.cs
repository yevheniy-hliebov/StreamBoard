using StreamBoard.Core;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Integrations.Twitch.ViewModels
{
    public partial class TwitchSettingsViewModel(TwitchAccountsGateway gateway) : ObservableObject
    {
        public TwitchAccountViewModel BroadcasterAccount { get; } = new TwitchAccountViewModel(gateway.Broadcaster);
        public TwitchAccountViewModel BotAccount { get; } = new TwitchAccountViewModel(gateway.Bot);
    }
}