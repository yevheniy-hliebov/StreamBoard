using System.Collections.ObjectModel;
using StreamBoard.Core;
using StreamBoard.Features.Integrations.Common.Models;
using StreamBoard.Features.Integrations.Obs.Models;
using StreamBoard.Features.Integrations.Obs.Services;

namespace StreamBoard.Features.Integrations.Common.ViewModels
{
    public class IntegrationsViewModel : ObservableObject
    {
        public ObservableCollection<IntegrationStateModel> Integrations { get; set; }

        public IntegrationsViewModel(ObsService obsService)
        {
            Integrations = [
                new ObsIntegrationState(obsService),
            ];
        }
    }
}