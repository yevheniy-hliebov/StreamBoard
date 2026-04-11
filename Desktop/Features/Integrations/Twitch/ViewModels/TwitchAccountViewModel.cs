using System.Windows.Input;
using StreamBoard.Core;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Integrations.Twitch.ViewModels
{
    public class TwitchAccountViewModel : ObservableObject
    {
        private readonly TwitchAccountManager _manager;

        private bool _isAuth;
        public bool IsAuth
        {
            get => _isAuth;
            set => SetProperty(ref _isAuth, value);
        }

        private TwitchUserIdentify? _user;
        public TwitchUserIdentify? User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public string AccountRole { get; }

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }

        public TwitchAccountViewModel(TwitchAccountManager manager)
        {
            _manager = manager;
            AccountRole = manager.Type.ToString();

            LoginCommand = new RelayCommand(_ => Login());
            LogoutCommand = new RelayCommand(_ => Logout());

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
}