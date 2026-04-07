using System.Diagnostics;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using StreamBoard.Features.Integrations.Twitch.Models;
using StreamBoard.Features.Integrations.Twitch.Services.Auth;

namespace StreamBoard.Features.Integrations.Twitch.Services
{
    public class TwitchAccountManager : IDisposable
    {
        public TwitchUserType Type { get; private set; }
        private readonly string _appClientId;
        private readonly List<string> _requiredScopes;
        private readonly IMemoryCache _cache;

        private readonly HttpClient _http;

        private TwitchAuthContext? _authContext;

        public TwitchUserIdentify? User { get; private set; }
        public TwitchApiClient? Api { get; private set; }

        public bool IsAuth => User != null;

        public event Action? UserChanged;

        private readonly System.Timers.Timer _pollTimer;

        private string? _cachedLoginState;

        public TwitchAccountManager(
            TwitchUserType type,
            List<string> scopes,
            string appClientId,
            IMemoryCache cache,
            HttpClient http)
        {
            Type = type;
            _requiredScopes = scopes;
            _appClientId = appClientId;
            _cache = cache;
            _http = http;

            _http.DefaultRequestHeaders.Add("Client-Id", _appClientId);

            _pollTimer = new System.Timers.Timer(60000);
            _pollTimer.Elapsed += async (sender, args) => await PollUser();
        }

        public void Login()
        {
            var builder = new TwitchAuthUriBuilder
            {
                ClientId = _appClientId,
                RedirectUri = $"http://localhost:13551/twitch/{Type.ToString().ToLower()}",
                ForceVerify = true,
                Scopes = _requiredScopes
            };

            _cachedLoginState = TwitchAuthUriBuilder.GenerateState();
            string authUrl = builder.Build(_cachedLoginState);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not open link: {ex.Message}");
            }
        }

        public async Task OnLoginSuccess(TwitchAuthContext context, string state)
        {
            _authContext = context;

            Api = new TwitchApiClient(context, _http, _cache);

            await PollUser();

            if (IsAuth)
            {
                _pollTimer.Start();
            }
        }

        public void Logout()
        {
            _pollTimer.Stop();
            _authContext = null;
            Api = null;

            if (User != null)
            {
                User = null;
                UserChanged?.Invoke();
            }
        }

        private async Task PollUser()
        {
            if (Api == null) return;

            try
            {
                var fetchedUser = await Api.Users.GetMe();

                if (fetchedUser != null)
                {
                    User = fetchedUser;
                    UserChanged?.Invoke();
                }
                else
                {
                    Logout();
                }
            }
            catch (Exception)
            {
                Logout();
            }
        }

        public void Dispose()
        {
            _pollTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}