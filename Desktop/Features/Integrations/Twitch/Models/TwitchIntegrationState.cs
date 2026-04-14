using StreamBoard.Features.Integrations.Common.Models;
using StreamBoard.Features.Integrations.Twitch.Services;
using StreamBoard.Features.Integrations.Twitch.Views.Pages;

namespace StreamBoard.Features.Integrations.Twitch.Models
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