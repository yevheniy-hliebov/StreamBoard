using StreamTabula.Features.Settings.Services;
using Wpf.Ui.Appearance;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Servers.Services;
using System.Windows;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Features.Integrations.Common.Services;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Servers.Controllers;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Updater.ViewModels;


namespace StreamTabula.Core.AppStartup
{
    public static class AppInitializer
    {
        private const string MutexName = "StreamTabula_SingleInstance_Mutex";
        private static Mutex? _mutex;

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // Mutex
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("StreamTabula is already up and running!", "StreamTabula", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }

            var settingStorage = serviceProvider.GetRequiredService<SettingsStorage>();
            var privilegeService = serviceProvider.GetRequiredService<PrivilegeService>();

            // Theme
            ApplicationThemeManager.Apply(
                settingStorage.Current.Theme == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light
            );

            // Run as admin
            if (settingStorage.Current.RunAsAdmin && !privilegeService.IsRunAsAdmin())
            {
                privilegeService.RestartAsAdmin();
                Application.Current.Shutdown();
                return;
            }

            // Check for updates
            var updater = serviceProvider.GetRequiredService<UpdaterViewModel>();
            _ = updater.CheckForUpdatesOnStartupAsync();

            // LocalServer start
            var LocalServer = serviceProvider.GetRequiredService<LocalServer>();
            if (LocalServer != null && LocalServer.ShouldAutoStart && !LocalServer.IsRunning)
                await LocalServer.Start();

            // OBS Connection
            var integrationStorage = serviceProvider.GetRequiredService<IntegrationConnectionStorage>();
            var obsService = serviceProvider.GetRequiredService<ObsService>();
            if (integrationStorage.Current.Obs.AutoConnectOnStartup && !obsService.IsConnected)
            {
                obsService.Connect();
            }

            // System Http Server
            var twitchAccountsGateway = serviceProvider.GetRequiredService<TwitchAccountsGateway>();
            var twitchController = new TwitchAuthController(twitchAccountsGateway);
            var httpRouter = new HttpRouter([twitchController]);
            var serverConfig = new LocalServerConfig { Port = 13551 };
            var systemServer = new LocalServer(serverConfig, httpRouter);

            await systemServer.Start();

            // Actions
            var registry = serviceProvider.GetRequiredService<ActionRegistry>();
            registry.RegisterActions();
        }
    }
}