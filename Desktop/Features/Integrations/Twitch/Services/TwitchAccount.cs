using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public interface ITwitchAccount : IDisposable
{
    ITwitchSession Session { get; }
    TwitchApiClient Api { get; }
    ITwitchAuthenticator Authenticator { get; }

    Task InitializeAsync();
}

public class TwitchAccount : ITwitchAccount
{
    public ITwitchSession Session { get; }
    public TwitchApiClient Api { get; }
    public ITwitchAuthenticator Authenticator { get; }

    private readonly ITwitchAuthContextStorage _storage;

    public TwitchAccount(
        ITwitchAuthContextStorage storage,
        TwitchAuthOptions options,
        IUrlLauncher urlLauncher,
        HttpClient http,
        IMemoryCache cache)
    {
        Session = new TwitchSession(options.Role);
        Api = new TwitchApiClient(Session, http, cache, options.ClientId);
        _storage = storage;
        Authenticator = new TwitchAuthenticator(options, Session, urlLauncher, Api);

        Session.SessionChanged += OnSessionChanged;
    }

    public async Task InitializeAsync()
    {
        var context = await _storage.LoadAsync();
        if (context != null)
        {
            await Authenticator.TryRestoreSessionAsync(context);
        }
    }

    private void OnSessionChanged()
    {
        if (Session.IsAuthenticated && Session.AuthContext != null)
        {
            _ = _storage.SaveAsync(Session.AuthContext);
        }
        else
        {
            _storage.Clean();
        }
    }

    public void Dispose()
    {
        Session.SessionChanged -= OnSessionChanged;
        Authenticator.Dispose();
        GC.SuppressFinalize(this);
    }
}