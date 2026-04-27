using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Services;
using StreamBoard.Features.Actions.Services;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Decks.ViewModels;
using StreamBoard.Features.Integrations.Common.Services;
using StreamBoard.Features.Integrations.Common.ViewModels;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Features.Integrations.Obs.ViewModels;
using StreamBoard.Features.Integrations.Twitch.Services;
using StreamBoard.Features.Integrations.Twitch.ViewModels;
using StreamBoard.Features.Servers.Controllers;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Servers.ViewModels;
using StreamBoard.Features.Settings.Services;
using StreamBoard.Features.Settings.ViewModels;
using StreamBoard.Features.Updater.Services;
using StreamBoard.Features.Updater.ViewModels;

namespace StreamBoard.Core.DI
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<PageService>();

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