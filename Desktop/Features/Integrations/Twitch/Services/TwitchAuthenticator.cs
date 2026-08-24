using System.Diagnostics;
using StreamTabula.Core.Services;
using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public interface ITwitchAuthenticator : IDisposable
{
    void StartLogin();
    Task FinalizeLogin(TwitchAuthContext context, string state);
    void Logout();
    Task TryRestoreSessionAsync(TwitchAuthContext context);
}

public class TwitchAuthenticator : ITwitchAuthenticator
{
    private readonly ITwitchSession _session;
    private readonly TwitchApiClient _api;
    private readonly IUrlLauncher _urlLauncher;
    private readonly TwitchAuthUriBuilder _builder;
    private readonly System.Timers.Timer _pollTimer;
    private string? _cachedLoginState;

    public TwitchAuthenticator(
        TwitchAuthOptions options,
        ITwitchSession session,
        IUrlLauncher urlLauncher,
        TwitchApiClient api)
    {
        _session = session;
        _api = api;
        _urlLauncher = urlLauncher;
        _builder = new TwitchAuthUriBuilder(options);

        _pollTimer = new System.Timers.Timer(60000);
        _pollTimer.Elapsed += async (sender, args) => await PollUserAsync();
    }

    public void StartLogin()
    {
        _cachedLoginState = TwitchAuthUriBuilder.GenerateState();
        string authUrl = _builder.Build(_cachedLoginState);

        try
        {
            _urlLauncher.OpenUrl(authUrl);
        }
        catch
        {
            _cachedLoginState = null;
            throw;
        }
    }

    public async Task FinalizeLogin(TwitchAuthContext context, string state)
    {
        if (string.IsNullOrWhiteSpace(_cachedLoginState) || _cachedLoginState != state)
        {
            _cachedLoginState = null;
            throw new InvalidOperationException("Invalid or expired OAuth state.");
        }
        _cachedLoginState = null;

        try
        {
            var user = await _api.Users.GetMe(overrideContext: context);

            if (user == null)
            {
                throw new InvalidOperationException("Failed to retrieve user profile.");
            }

            _session.SetSession(context, user);

            _pollTimer.Start();
        }
        catch (Exception)
        {
            Logout();
            throw;
        }
    }

    private async Task PollUserAsync()
    {
        if (!_session.IsAuthenticated) return;

        try
        {
            var fetchedUser = await _api.Users.GetMe();
            if (fetchedUser != null)
            {
                _session.SetSession(_session.AuthContext!, fetchedUser);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TwitchPoll] Transient error updating user info: {ex.Message}");
        }
    }

    public void Logout()
    {
        _pollTimer.Stop();
        _session.Clear();
    }

    public async Task TryRestoreSessionAsync(TwitchAuthContext context)
    {
        try
        {
            var user = await _api.Users.GetMe(overrideContext: context);

            if (user != null)
            {
                _session.SetSession(context, user);
                _pollTimer.Start();
            }
            else
            {
                Logout();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Twitch] Failed to restore session: {ex.Message}");
            Logout();
        }
    }

    public void Dispose()
    {
        _pollTimer.Stop();
        _pollTimer.Dispose();
        GC.SuppressFinalize(this);
    }
}