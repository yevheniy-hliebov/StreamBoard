using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Features.Integrations.Obs.Views.Pages;

namespace StreamTabula.Features.Integrations.Obs.Models
{
    public class ObsIntegrationState : IntegrationStateModel
    {
        private readonly ObsService _obsService;

        public ObsIntegrationState(ObsService obsService)
        {
            _obsService = obsService;
            Name = "OBS Studio";

            State = _obsService.ConnectionState;

            TargetPageType = typeof(ObsSettingsPage);

            _obsService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ObsService.ConnectionState))
                {
                    State = _obsService.ConnectionState;
                }
            };
        }
    }
}