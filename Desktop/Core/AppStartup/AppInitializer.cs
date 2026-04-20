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


namespace StreamBoard.Core.AppStartup
{
    public static class AppInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var settingStorage = serviceProvider.GetRequiredService<SettingsStorage>();
            var privilegeService = serviceProvider.GetRequiredService<PrivilegeService>();
            var httpServer = serviceProvider.GetRequiredService<HttpServer>();
            var registry = serviceProvider.GetRequiredService<ActionRegistry>();
            var gridDeckStorage = serviceProvider.GetRequiredService<GridDeckStorage>();
            var keyboardDeckStorage = serviceProvider.GetRequiredService<KeyboardDeckStorage>();

            var integrationStorage = serviceProvider.GetRequiredService<IntegrationConnectionStorage>();
            var obsService = serviceProvider.GetRequiredService<ObsService>();

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

            // HttpServer start
            if (httpServer != null && httpServer.ShouldAutoStart && !httpServer.IsRunning)
                await httpServer.Start();

            // OBS Connection
            if (integrationStorage.Current.Obs.AutoConnectOnStartup && !obsService.IsConnected)
            {
                obsService.Connect();
            }

            // System Http Server
            var twitchAccountsGateway = serviceProvider.GetRequiredService<TwitchAccountsGateway>();
            var twitchController = new TwitchAuthController(twitchAccountsGateway);
            var httpRouter = new HttpRouter([twitchController]);
            var serverConfig = new HttpServerConfig { Port = 13551 };
            var systemServer = new HttpServer(serverConfig, httpRouter);

            await systemServer.Start();

            // Actions
            registry.RegisterActions();

            // Decks
            gridDeckStorage.Initialize();
            keyboardDeckStorage.Initialize();
        }
    }
}