using StreamTabula.Features.Integrations.Twitch.Models;

namespace StreamTabula.Features.Integrations.Twitch.Services;

public interface ITwitchSession
{
    TwitchAccountRole Role { get; }
    TwitchAuthContext? AuthContext { get; }
    TwitchUserIdentity? User { get; }

    bool IsAuthenticated => AuthContext != null && User != null;

    void SetSession(TwitchAuthContext authContext, TwitchUserIdentity user);
    void Clear();

    event Action? SessionChanged;
}

public class TwitchSession(TwitchAccountRole role) : ITwitchSession
{
    public TwitchAccountRole Role { get; } = role;
    public TwitchAuthContext? AuthContext { get; private set; }
    public TwitchUserIdentity? User { get; private set; }

    public event Action? SessionChanged;

    public void SetSession(TwitchAuthContext authContext, TwitchUserIdentity user)
    {
        AuthContext = authContext ?? throw new ArgumentNullException(nameof(authContext));
        User = user ?? throw new ArgumentNullException(nameof(user));

        SessionChanged?.Invoke();
    }

    public void Clear()
    {
        AuthContext = null;
        User = null;

        SessionChanged?.Invoke();
    }
}
