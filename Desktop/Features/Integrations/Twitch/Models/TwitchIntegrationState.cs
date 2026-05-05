using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Integrations.Twitch.Views.Pages;

namespace StreamTabula.Features.Integrations.Twitch.Models
{
    public class TwitchIntegrationState : IntegrationStateModel
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
            State = _manager.IsAuth
                ? ConnectionState.Connected
                : ConnectionState.NotConnected;
        }
    }
}