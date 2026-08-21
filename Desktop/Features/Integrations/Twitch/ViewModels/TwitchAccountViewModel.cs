using StreamTabula.Core.Mvvm;
using StreamTabula.Features.Integrations.Twitch.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using Wpf.Ui.Input;

namespace StreamTabula.Features.Integrations.Twitch.ViewModels;

public class TwitchAccountViewModel : ObservableObject
{
    private readonly TwitchAccountManager _manager;

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

    public TwitchAccountViewModel(TwitchAccountManager manager)
    {
        _manager = manager;
        AccountRole = manager.Options.Role.ToString();

        LoginCommand = new RelayCommand<object?>(_ => Login());
        LogoutCommand = new RelayCommand<object?>(_ => Logout());

        _manager.UserChanged += UpdateState;
        UpdateState();
    }

    private void UpdateState()
    {
        IsAuth = _manager.IsAuth;
        User = IsAuth ? _manager.User : null;
    }

    private void Login() => _manager.Login();

    private void Logout() => _manager.Logout();
}