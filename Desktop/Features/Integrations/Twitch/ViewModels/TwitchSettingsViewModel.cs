using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Integrations.Twitch.ViewModels
{
    public partial class TwitchSettingsViewModel(ITwitchAccountsGateway gateway) : ObservableObject
    {
        public TwitchAccountViewModel BroadcasterAccount { get; } = new TwitchAccountViewModel(gateway.Broadcaster);
        public TwitchAccountViewModel BotAccount { get; } = new TwitchAccountViewModel(gateway.Bot);
    }
}