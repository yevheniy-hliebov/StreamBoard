using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Services;
using StreamTabula.Features.Actions.Services;
using StreamTabula.Features.Decks.Services;
using StreamTabula.Features.Decks.ViewModels;
using StreamTabula.Features.Integrations.Common.Services;
using StreamTabula.Features.Integrations.Common.ViewModels;
using StreamTabula.Features.Integrations.Obs.Services;
using StreamTabula.Features.Integrations.Obs.ViewModels;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Features.Integrations.Twitch.ViewModels;
using StreamTabula.Features.Servers.Controllers;
using StreamTabula.Features.Servers.Services;
using StreamTabula.Features.Servers.ViewModels;
using StreamTabula.Features.Settings.Services;
using StreamTabula.Features.Settings.ViewModels;
using StreamTabula.Features.Updater.Services;
using StreamTabula.Features.Updater.ViewModels;
using System.Net.Http;
using Wpf.Ui;

namespace StreamTabula.Core.DI
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<Features.Navigation.Services.NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton<SettingsStorage>();
            services.AddSingleton<ServerConfigsStorage>();
            services.AddSingleton<StartupService>();
            services.AddSingleton<PrivilegeService>();
            services.AddSingleton<GridDeckStorage>();
            services.AddSingleton<KeyboardDeckStorage>();
            services.AddSingleton<ActionRegistry>();

            services.AddSingleton<IntegrationConnectionStorage>();

            services.AddSingleton<ObsService>(sp =>
            {
                var storage = sp.GetRequiredService<IntegrationConnectionStorage>();

                var obsSettings = storage.Current.Obs;

                return new ObsService(obsSettings);
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
            services.AddSingleton<ObsSettingsViewModel>();
            services.AddSingleton<TwitchSettingsViewModel>();
            services.AddSingleton<LocalServerViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<UpdaterViewModel>();
        }
    }
}