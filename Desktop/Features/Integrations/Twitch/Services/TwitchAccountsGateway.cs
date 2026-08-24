using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Twitch.Models;
using System.IO;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public interface ITwitchAccountsGateway : IDisposable
{
    ITwitchAccount Broadcaster { get; }
    ITwitchAccount Bot { get; }
}

public class TwitchAccountsGateway : ITwitchAccountsGateway
{
    public ITwitchAccount Broadcaster { get; }
    public ITwitchAccount Bot { get; }

    public TwitchAccountsGateway(
        IMemoryCache cache,
        HttpClient http,
        IUrlLauncher urlLauncher,
        string appClientId
    )
    {
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

        var broadcasterOptions = new TwitchAuthOptions
        {
            ClientId = appClientId,
            Role = TwitchAccountRole.Broadcaster,
            Scopes = [
                "user:read:email",
                "channel:manage:broadcast",
                "user:write:chat",
                "moderator:manage:chat_messages",
                "moderator:manage:announcements",
                "moderator:manage:shoutouts",
                "moderator:manage:chat_settings",
                "channel:manage:broadcast",
                "clips:edit",
                "channel:edit:commercial",
                "moderator:manage:shield_mode",
            ],
        };

        var broadcasterStorage = new TwitchAuthContextStorage(dataDirectory, TwitchAccountRole.Broadcaster);
        Broadcaster = new TwitchAccount(broadcasterStorage, broadcasterOptions, urlLauncher, http, cache);

        var botOptions = new TwitchAuthOptions
        {
            ClientId = appClientId,
            Role = TwitchAccountRole.Bot,
            Scopes = [
                "user:read:email",
                "user:write:chat",
                "moderator:manage:announcements",
            ],
        };

        var botStorage = new TwitchAuthContextStorage(dataDirectory, TwitchAccountRole.Bot);
        Bot = new TwitchAccount(botStorage, botOptions, urlLauncher, http, cache);

        Broadcaster.Session.SessionChanged += CheckForDuplicateAccounts;
        Bot.Session.SessionChanged += CheckForDuplicateAccounts;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await Broadcaster.InitializeAsync();
        await Bot.InitializeAsync();
    }

    private void CheckForDuplicateAccounts()
    {
        if (!Broadcaster.Session.IsAuthenticated || !Bot.Session.IsAuthenticated) return;

        if (Broadcaster.Session.User!.Id == Bot.Session.User!.Id)
        {
            Bot.Authenticator.Logout();
        }
    }

    public void Dispose()
    {
        Broadcaster.Session.SessionChanged -= CheckForDuplicateAccounts;
        Bot.Session.SessionChanged -= CheckForDuplicateAccounts;

        Broadcaster.Dispose();
        Bot.Dispose();

        GC.SuppressFinalize(this);
    }
}