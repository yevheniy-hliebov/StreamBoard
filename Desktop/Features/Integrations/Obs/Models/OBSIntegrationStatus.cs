using System.Windows;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Features.Integrations.Obs.Views.Pages;

namespace StreamTabula.Features.Integrations.Obs.Models;

public class OBSIntegrationStatus : IntegrationStatusModel, IDisposable
{
    private readonly IOBSConnectionService _obsService;

    public OBSIntegrationStatus(IOBSConnectionService obsService)
    {
        _obsService = obsService;
        Name = "OBS Studio";

        Status = _obsService.Status;

        TargetPageType = typeof(ObsSettingsPage);

        _obsService.StatusChanged += OnObsStatusChanged;
    }

    private void OnObsStatusChanged(ConnectionStatus newStatus)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Status = newStatus;
        });
    }

    public void Dispose()
    {
        _obsService.StatusChanged -= OnObsStatusChanged;
    }
}