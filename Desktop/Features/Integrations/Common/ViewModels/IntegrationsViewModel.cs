using System.Collections.ObjectModel;
using StreamBoard.Core;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Integrations.Common.ViewModels
{
    public class IntegrationsViewModel : ObservableObject
    {
        public ObservableCollection<IntegrationStateModel> Integrations { get; set; }

        public IntegrationsViewModel()
        {
            Integrations = [
                new IntegrationStateModel { Name = "OBS Studio", State = ConnectionState.Connected },
                new IntegrationStateModel { Name = "Twitch", State = ConnectionState.Connecting },
                new IntegrationStateModel { Name = "Streamer.bot", State = ConnectionState.Failed },
                new IntegrationStateModel { Name = "YouTube", State = ConnectionState.NotConnected }
            ];
        }
    }
}