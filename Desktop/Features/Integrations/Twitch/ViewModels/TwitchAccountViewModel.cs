using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Integrations.Twitch.ViewModels;

public class TwitchAccountViewModel : ObservableObject
{
    private readonly ITwitchAccount _account;

    private bool _isAuth;
    public bool IsAuth
    {
        get => _isAuth;
        set => SetProperty(ref _isAuth, value);
    }

    private TwitchUserIdentity? _user;
    public TwitchUserIdentity? User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    public string AccountRole { get; }

    public IRelayCommand<object?> LoginCommand { get; }
    public IRelayCommand<object?> LogoutCommand { get; }

    public TwitchAccountViewModel(ITwitchAccount account)
    {
        _account = account;
        AccountRole = account.Session.Role.ToString();

        LoginCommand = new RelayCommand<object?>(_ => Login());
        LogoutCommand = new RelayCommand<object?>(_ => Logout());

        _account.Session.SessionChanged += UpdateState;
        UpdateState();
    }

    private void UpdateState()
    {
        IsAuth = _account.Session.IsAuthenticated;
        User = IsAuth ? _account.Session.User : null;
    }

    private void Login() => _account.Authenticator.StartLogin();

    private void Logout() => _account.Authenticator.Logout();
}