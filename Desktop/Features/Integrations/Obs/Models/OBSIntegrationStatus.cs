using System.Windows;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.OBS.Services;
using StreamTabula.Features.Integrations.OBS.Views.Pages;

namespace StreamTabula.Features.Integrations.OBS.Models;

public class OBSIntegrationStatus : IntegrationStatusModel, IDisposable
{
    private readonly IOBSConnectionService _obsService;

    public OBSIntegrationStatus(IOBSConnectionService obsService)
    {
        _obsService = obsService;
        Name = "OBS Studio";

        Status = _obsService.Status;

        TargetPageType = typeof(OBSSettingsPage);

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