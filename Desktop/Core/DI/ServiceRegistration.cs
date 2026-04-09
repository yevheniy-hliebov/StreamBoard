using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Decks.ViewModels;
using StreamBoard.Features.Integrations.Common.Services;
using StreamBoard.Features.Integrations.Common.ViewModels;
using StreamBoard.Features.Integrations.Obs.Services;
using StreamBoard.Features.Integrations.Obs.ViewModels;
using StreamBoard.Features.Servers.Controllers;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Servers.ViewModels;
using StreamBoard.Features.Settings.Services;
using StreamBoard.Features.Settings.ViewModels;

namespace StreamBoard.Core.DI
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<SettingsStorage>();
            services.AddSingleton<ServerConfigsStorage>();
            services.AddSingleton<StartupService>();
            services.AddSingleton<PrivilegeService>();
            services.AddSingleton<GridDeckStorage>();
            services.AddSingleton<KeyboardDeckStorage>();
            services.AddSingleton<HttpServerViewModel>();
            services.AddSingleton<ActionRegistry>();

            services.AddSingleton<IntegrationConnectionStorage>();

            services.AddSingleton<ObsService>(sp =>
            {
                var storage = sp.GetRequiredService<IntegrationConnectionStorage>();

                var obsSettings = storage.Current.Obs;

                return new ObsService(obsSettings);
            });


            services.AddSingleton<HttpServer>(sp =>
            {
                var storage = sp.GetRequiredService<ServerConfigsStorage>();
                var homeController = new HomeController(storage.Current.Http);
                var gridDeckStorage = sp.GetRequiredService<GridDeckStorage>();
                var gridDeckController = new GridDeckController(gridDeckStorage);
                var httpRouter = new HttpRouter([homeController, gridDeckController]);

                return new HttpServer(storage.Current.Http, httpRouter);
            });

            services.AddSingleton<IntegrationsViewModel>();
            services.AddTransient<GridDeckViewModel>();
            services.AddTransient<ObsSettingsViewModel>();
            services.AddTransient<SettingsViewModel>();
        }
    }
}