using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public class TwitchAccountsGateway : IDisposable
{
    private readonly TwitchStorageService _storage;
    public ITwitchAccountManager Broadcaster { get; }
    public ITwitchAccountManager Bot { get; }

    public TwitchAccountsGateway(
        IMemoryCache cache,
        HttpClient http,
        TwitchStorageService storage,
        IUrlLauncher urlLauncher, 
        string appClientId
    )
    {
        _storage = storage;

        var broadcasterOptions = new TwitchAuthOptions
        {
            ClientId = appClientId,
            Role = TwitchAccountRole.Broadcaster,
            Scopes = [
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
            ],
        };

        var broadcasterSession = new TwitchSession(TwitchAccountRole.Broadcaster);
        var broadcasterApi = new TwitchApiClient(broadcasterSession, http, cache);

        Broadcaster = new TwitchAccountManager(broadcasterOptions, broadcasterSession, urlLauncher, broadcasterApi);

        var botOptions = new TwitchAuthOptions
        {
            ClientId = appClientId,
            Role = TwitchAccountRole.Bot,
            Scopes = [
                "user:read:email",                  // Email in Get Users
                "user:write:chat",                  // Send Chat Message
                "moderator:manage:announcements",   // Send Chat Announcement
            ],
        };

        var botSession = new TwitchSession(TwitchAccountRole.Bot);
        var botApi = new TwitchApiClient(botSession, http, cache);

        Bot = new TwitchAccountManager(botOptions, botSession, urlLauncher, botApi);

        Broadcaster.Session.SessionChanged += OnBroadcasterSessionChanged;
        Bot.Session.SessionChanged += OnBotSessionChanged;

        _ = RestoreSessionsAsync();
    }

    private async Task RestoreSessionsAsync()
    {
        var broadcasterContext = _storage.LoadContext(TwitchAccountRole.Broadcaster);
        if (broadcasterContext != null)
        {
            await Broadcaster.TryRestoreSessionAsync(broadcasterContext);
        }

        var botContext = _storage.LoadContext(TwitchAccountRole.Bot);
        if (botContext != null)
        {
            await Bot.TryRestoreSessionAsync(botContext);
        }
    }

    private void OnBroadcasterSessionChanged() => SyncWithStorage(Broadcaster);
    private void OnBotSessionChanged() => SyncWithStorage(Bot);

    private void SyncWithStorage(ITwitchAccountManager manager)
    {
        if (manager.Session.IsAuthenticated)
        {
            _storage.SaveContext(manager.Session.Role, manager.Session.AuthContext!);
        }
        else
        {
            _storage.DeleteContext(manager.Session.Role);
        }

        CheckForDuplicateAccounts();
    }

    private void CheckForDuplicateAccounts()
    {
        if (!Broadcaster.Session.IsAuthenticated || !Bot.Session.IsAuthenticated) return;

        if (Broadcaster.Session.User!.Id == Bot.Session.User!.Id)
        {
            Bot.Logout();
        }
    }

    public void Dispose()
    {
        Broadcaster.Session.SessionChanged -= OnBroadcasterSessionChanged;
        Bot.Session.SessionChanged -= OnBotSessionChanged;

        Broadcaster.Dispose();
        Bot.Dispose();

        GC.SuppressFinalize(this);
    }
}