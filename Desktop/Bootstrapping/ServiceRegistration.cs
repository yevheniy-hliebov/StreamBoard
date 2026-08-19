using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Services;

using StreamTabula.Features.Decks.Services;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Integrations.Common.Services;
using StreamTabula.Features.Integrations.OBS.Services;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Servers.Services;
using StreamTabula.Features.Updater.Services;

using StreamTabula.Features.Home.Views.Pages;
using StreamTabula.Features.Decks.Views.Pages;
using StreamTabula.Features.Integrations.Common.Views.Pages;
using StreamTabula.Features.Integrations.OBS.Views.Pages;
using StreamTabula.Features.Integrations.Twitch.Views.Pages;
using StreamTabula.Features.Servers.Pages;
using StreamTabula.Features.Settings.Views.Pages;

using StreamTabula.Features.Decks.ViewModels;
using StreamTabula.Features.Integrations.Common.ViewModels;
using StreamTabula.Features.Integrations.OBS.ViewModels;
using StreamTabula.Features.Integrations.Twitch.ViewModels;
using StreamTabula.Features.Servers.ViewModels;
using StreamTabula.Features.Settings.ViewModels;
using StreamTabula.Features.Updater.ViewModels;

using StreamTabula.Features.Servers.Controllers;

using System.Net.Http;
using Wpf.Ui;
using OBSWebsocketDotNet;

namespace StreamTabula.Bootstrapping;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();

        services.AddSingleton<Features.Navigation.Services.NavigationService>();

        // Pages
        services.AddTransient<HomePage>();
        services.AddTransient<GridDeckPage>();
        services.AddTransient<IntegrationsPage>();
        services.AddTransient<OBSSettingsPage>();
        services.AddTransient<TwitchSettingsPage>();
        services.AddTransient<LocalServerPage>();
        services.AddTransient<SettingsPage>();

        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<SettingsStorage>();
        services.AddSingleton<ServerConfigsStorage>();
        services.AddSingleton<GridDeckStorage>();
        services.AddSingleton<KeyboardDeckStorage>();
        services.AddSingleton<ActionRegistry>();

        services.AddSingleton<IntegrationsStorage>();
        services.AddSingleton<IOBSWebsocket, OBSWebsocket>();
        services.AddSingleton<IOBSSceneService, OBSSceneService>();

        services.AddSingleton<IOBSConnectionService>(sp =>
        {
            var obs = sp.GetRequiredService<IOBSWebsocket>();

            var storage = sp.GetRequiredService<IntegrationsStorage>();
            var obsSettings = storage.Current.Obs;

            return new OBSConnectionService(obs, obsSettings);
        });

        services.AddMemoryCache();
        services.AddSingleton<HttpClient>();
        services.AddSingleton<TwitchStorageService>();
        services.AddSingleton<TwitchAccountsGateway>(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var http = sp.GetRequiredService<HttpClient>();
            var storage = sp.GetRequiredService<TwitchStorageService>();

            string clientId = "0sn4idtk1x80yrrej0cd66nsapzv86";

            return new TwitchAccountsGateway(cache, http, storage, clientId);
        });


        services.AddSingleton<WebsocketManager>();
        services.AddSingleton<LocalServer>(sp =>
        {
            var storage = sp.GetRequiredService<ServerConfigsStorage>();
            var homeController = new HomeController(storage.Current.Local);
            var gridDeckStorage = sp.GetRequiredService<GridDeckStorage>();
            var gridDeckController = new GridDeckController(gridDeckStorage);
            var httpRouter = new HttpRouter([homeController, gridDeckController]);
            var wsManager = sp.GetRequiredService<WebsocketManager>();

            return new LocalServer(storage.Current.Local, httpRouter, wsManager);
        });

        services.AddSingleton<AppInfoService>();
        services.AddSingleton<UpdateService>();

        services.AddSingleton<GridDeckViewModel>();
        services.AddSingleton<IntegrationsViewModel>();
        services.AddSingleton<OBSSettingsViewModel>();
        services.AddSingleton<TwitchSettingsViewModel>();
        services.AddSingleton<LocalServerViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<UpdaterViewModel>();
    }
}