using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamBoard.Features.Integrations.Twitch.Models;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchAccountsGateway : IDisposable
    {
        public TwitchAccountManager Broadcaster { get; }
        public TwitchAccountManager Bot { get; }

        public TwitchAccountsGateway(IMemoryCache cache, HttpClient http)
        {
            string appClientId = "";

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

            Broadcaster.UserChanged += CheckForDuplicateAccounts;
            Bot.UserChanged += CheckForDuplicateAccounts;
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