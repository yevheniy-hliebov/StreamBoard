using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Integrations.Common.Services;
using StreamTabula.Features.Integrations.OBS.Services;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Servers.Controllers;
using StreamTabula.Features.Servers.Models;
using StreamTabula.Features.Servers.Services;
using StreamTabula.Features.Settings.Helpers;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Updater.ViewModels;
using System.Net;
using System.Windows;
using Wpf.Ui.Appearance;

namespace StreamTabula.Bootstrapping;

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
            return;
        }

        var settingStorage = serviceProvider.GetRequiredService<SettingsStorage>();

        // Theme
        ApplicationThemeManager.Apply(
            settingStorage.Current.Appearance.Theme == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light
        );

        // Run as admin
        if (settingStorage.Current.Startup.RunAsAdmin && !AdminPrivilegeHelper.IsRunningAsAdministrator())
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
                _mutex = null;
            }

            AdminPrivilegeHelper.RestartAsAdministrator();
            Application.Current.Shutdown();
            return;
        }

        // Check for updates
        var updater = serviceProvider.GetRequiredService<UpdaterViewModel>();
        _ = updater.CheckForUpdatesOnStartupAsync();

        // 5. LocalServer start
        await LocalServerBootstrapper.EnsureStartedAsync(serviceProvider);


        // OBS Connection
        var integrationStorage = serviceProvider.GetRequiredService<IntegrationsStorage>();
        var obsService = serviceProvider.GetRequiredService<IOBSConnectionService>();

        if (integrationStorage.Current.Obs.AutoConnectOnStartup && !obsService.IsConnected)
        {
            await obsService.ConnectAsync();
        }

        // System Http Server
        var twitchAccountsGateway = serviceProvider.GetRequiredService<ITwitchAccountsGateway>();
        var twitchController = new TwitchAuthController(twitchAccountsGateway);
        var httpRouter = new HttpRouter([twitchController]);

        var serverConfig = new LocalServerConfig { Port = 13551 };
        var systemServer = new LocalServer(IPAddress.Loopback, serverConfig, httpRouter);

        await systemServer.StartAsync();

        // Actions
        var registry = serviceProvider.GetRequiredService<ActionRegistry>();
        registry.RegisterActions();
    }
}