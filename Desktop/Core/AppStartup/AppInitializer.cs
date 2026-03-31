using StreamBoard.Features.Settings.Services;
using Wpf.Ui.Appearance;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Decks.Services;
using System.Windows;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Features.Integrations.Common.Services;

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

            if (integrationStorage.Current.Obs.AutoConnectOnStartup && !obsService.IsConnected)
            {
                obsService.Connect();
            }

            // Actions
            registry.RegisterActions();

            // Decks
            gridDeckStorage.Initialize();
            keyboardDeckStorage.Initialize();
        }
    }
}