using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Integrations.Twitch.Views.Pages;

namespace StreamTabula.Features.Integrations.Twitch.Models
{
    public class TwitchIntegrationState : IntegrationStatusModel
    {
        private readonly ITwitchAccount _account;

        public TwitchIntegrationState(ITwitchAccount manager)
        {
            _account = manager;

            Name = $"Twitch {manager.Session.Role}";
            TargetPageType = typeof(TwitchSettingsPage);

            UpdateStatus();

            _account.Session.SessionChanged += UpdateStatus;
        }

        private void UpdateStatus()
        {
            Status = _account.Session.IsAuthenticated
                ? ConnectionStatus.Connected
                : ConnectionStatus.NotConnected;
        }
    }
}