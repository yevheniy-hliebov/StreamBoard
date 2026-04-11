using System.Collections.ObjectModel;
using StreamBoard.Core;
using StreamBoard.Features.Integrations.Common.Models;
using StreamBoard.Features.Integrations.Obs.Models;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Integrations.Common.ViewModels
{
    public class IntegrationsViewModel : ObservableObject
    {
        public ObservableCollection<IntegrationStateModel> Integrations { get; set; }

        public IntegrationsViewModel(ObsService obsService, TwitchAccountsGateway twitchGateway)
        {
            Integrations = [
                new ObsIntegrationState(obsService),
                new TwitchIntegrationState(twitchGateway.Broadcaster),
                new TwitchIntegrationState(twitchGateway.Bot)
            ];
        }
    }
}