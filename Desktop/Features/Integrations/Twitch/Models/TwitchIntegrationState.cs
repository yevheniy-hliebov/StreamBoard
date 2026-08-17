using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Integrations.Twitch.Views.Pages;

namespace StreamTabula.Features.Integrations.Twitch.Models
{
    public class TwitchIntegrationState : IntegrationStatusModel
    {
        private readonly TwitchAccountManager _manager;

        public TwitchIntegrationState(TwitchAccountManager manager)
        {
            _manager = manager;

            Name = $"Twitch {manager.Type}";
            TargetPageType = typeof(TwitchSettingsPage);

            UpdateStatus();

            _manager.UserChanged += UpdateStatus;
        }

        private void UpdateStatus()
        {
            Status = _manager.IsAuth
                ? ConnectionStatus.Connected
                : ConnectionStatus.NotConnected;
        }
    }
}