using System.Collections.ObjectModel;
using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Obs.Models;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Integrations.Common.ViewModels;

public class IntegrationsViewModel : ObservableObject
{
    public ObservableCollection<IntegrationStatusModel> IntegrationConnectionStatus { get; set; }

    public IntegrationsViewModel(ObsService obsService, TwitchAccountsGateway twitchGateway)
    {
        IntegrationConnectionStatus = [
            new ObsIntegrationState(obsService),
            new TwitchIntegrationState(twitchGateway.Broadcaster),
            new TwitchIntegrationState(twitchGateway.Bot)
        ];
    }
}