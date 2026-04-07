using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamBoard.Features.Integrations.Twitch.Models;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchAccountsGateway : IDisposable
    {
        private readonly TwitchStorageService _storage;
        public TwitchAccountManager Broadcaster { get; }
        public TwitchAccountManager Bot { get; }

        public TwitchAccountsGateway(
            IMemoryCache cache,
            HttpClient http,
            TwitchStorageService storage,
            string appClientId
        )
        {
            _storage = storage;

            var broadcasterScopes = new List<string> {
                "user:read:email",                  // Email in Get Users
                "channel:manage:broadcast",         // Modify Channel Information
                "user:write:chat",                  // Send Chat Message
                "moderator:manage:chat_messages",   // for Delete Chat Messages
                "moderator:manage:announcements",   // Send Chat Announcement
                "moderator:manage:shoutouts",       // Send a Shoutout
                "moderator:manage:chat_settings",   // Update Chat Settings
                "channel:manage:broadcast",         // Create Stream Marker
                "clips:edit",                       // Create Clip
                "channel:edit:commercial",          // Start Commercial
                "moderator:manage:shield_mode",     // Update Shield Mode Status
            };

            Broadcaster = new TwitchAccountManager(
                type: TwitchUserType.Broadcaster,
                scopes: broadcasterScopes,
                appClientId: appClientId,
                cache: cache,
                http: http
            );

            var botScopes = new List<string> {
                "user:read:email",                  // Email in Get Users
                "user:write:chat",                  // Send Chat Message
                "moderator:manage:announcements",   // Send Chat Announcement
            };
            Bot = new TwitchAccountManager(
                type: TwitchUserType.Bot,
                scopes: botScopes,
                appClientId: appClientId,
                cache: cache,
                http: http
            );

            RestoreSessions();

            Broadcaster.UserChanged += () => SyncWithStorage(Broadcaster);
            Bot.UserChanged += () => SyncWithStorage(Bot);
        }

        private void RestoreSessions()
        {
            var broadcasterContext = _storage.LoadContext(TwitchUserType.Broadcaster);
            if (broadcasterContext != null)
            {
                _ = Broadcaster.OnLoginSuccess(broadcasterContext, "restored");
            }

            var botContext = _storage.LoadContext(TwitchUserType.Bot);
            if (botContext != null)
            {
                _ = Bot.OnLoginSuccess(botContext, "restored");
            }
        }

        private void SyncWithStorage(TwitchAccountManager manager)
        {
            if (manager.IsAuth && manager.AuthContext != null)
            {
                _storage.SaveContext(manager.Type, manager.AuthContext);
            }
            else
            {
                _storage.DeleteContext(manager.Type);
            }

            CheckForDuplicateAccounts();
        }

        private void CheckForDuplicateAccounts()
        {
            if (!Broadcaster.IsAuth || !Bot.IsAuth) return;

            if (Broadcaster.User!.Id == Bot.User!.Id)
            {
                Bot.Logout();
            }
        }

        public void Dispose()
        {
            Broadcaster.UserChanged -= CheckForDuplicateAccounts;
            Bot.UserChanged -= CheckForDuplicateAccounts;

            Broadcaster.Dispose();
            Bot.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}