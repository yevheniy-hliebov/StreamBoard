using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.Decks.ViewModels;
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

            services.AddSingleton<HttpServer>(sp =>
            {
                var storage = sp.GetRequiredService<ServerConfigsStorage>();
                var homeController = new HomeController(storage.Current.Http);
                var gridDeckStorage = sp.GetRequiredService<GridDeckStorage>();
                var gridDeckController = new GridDeckController(gridDeckStorage);
                var httpRouter = new HttpRouter([homeController, gridDeckController]);

                return new HttpServer(storage.Current.Http, httpRouter);
            });

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<GridDeckViewModel>();
        }
    }
}