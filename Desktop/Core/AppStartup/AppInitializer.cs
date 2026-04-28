using StreamBoard.Features.Settings.Services;
using Wpf.Ui.Appearance;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Decks.Services;
using System.Windows;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Features.Integrations.Common.Services;
using StreamBoard.Features.Integrations.Twitch.Services;
using StreamBoard.Features.Servers.Controllers;
using StreamBoard.Features.Servers.Models;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Updater.ViewModels;


namespace StreamBoard.Core.AppStartup
{
    public static class AppInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
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

            // Decks
            var gridDeckStorage = serviceProvider.GetRequiredService<GridDeckStorage>();
            gridDeckStorage.Initialize();
        }
    }
}