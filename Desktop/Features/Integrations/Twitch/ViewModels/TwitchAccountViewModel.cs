using System.Windows.Input;
using StreamBoard.Core;
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

        private string? _displayName;
        public string? DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private string? _profileImageUrl;
        public string? ProfileImageUrl
        {
            get => _profileImageUrl;
            set => SetProperty(ref _profileImageUrl, value);
        }

        private string? _broadcasterType;
        public string? BroadcasterType
        {
            get => _broadcasterType;
            set => SetProperty(ref _broadcasterType, value);
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

            if (IsAuth && _manager.User != null)
            {
                DisplayName = _manager.User.DisplayName;
                ProfileImageUrl = _manager.User.ProfileImageUrl;

                BroadcasterType = string.IsNullOrEmpty(_manager.User.BroadcasterType)
                    ? "Standard"
                    : char.ToUpper(_manager.User.BroadcasterType[0]) + _manager.User.BroadcasterType.Substring(1);
            }
            else
            {
                DisplayName = null;
                ProfileImageUrl = null;
                BroadcasterType = null;
            }
        }

        private void Login() => _manager.Login();

        private void Logout() => _manager.Logout();
    }
}