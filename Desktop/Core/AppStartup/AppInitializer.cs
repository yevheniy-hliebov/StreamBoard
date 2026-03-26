using StreamBoard.Features.Settings.Services;
using Wpf.Ui.Appearance;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Decks.Services;
using System.Windows;

namespace StreamBoard.Core.AppStartup
{
    public static class AppInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var storage = serviceProvider.GetRequiredService<SettingsStorage>();
            var privilegeService = serviceProvider.GetRequiredService<PrivilegeService>();
            var httpServer = serviceProvider.GetRequiredService<HttpServer>();
            var registry = serviceProvider.GetRequiredService<ActionRegistry>();
            var gridDeckStorage = serviceProvider.GetRequiredService<GridDeckStorage>();
            var keyboardDeckStorage = serviceProvider.GetRequiredService<KeyboardDeckStorage>();

            // Theme
            ApplicationThemeManager.Apply(
                storage.Current.Theme == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light
            );

            // Run as admin
            if (storage.Current.RunAsAdmin && !privilegeService.IsRunAsAdmin())
            {
                privilegeService.RestartAsAdmin();
                Application.Current.Shutdown();
                return;
            }

            // HttpServer start
            if (httpServer != null && httpServer.ShouldAutoStart && !httpServer.IsRunning)
                await httpServer.Start();

            // Actions
            registry.RegisterActions();

            // Decks
            gridDeckStorage.Initialize();
            keyboardDeckStorage.Initialize();
        }
    }
}