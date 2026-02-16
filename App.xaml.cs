using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Decks.Services;
using StreamBoard.Features.GridDeck.ViewModels;
using StreamBoard.Features.Servers.Controllers;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Servers.ViewModels;
using StreamBoard.Features.Settings.Services;
using StreamBoard.Features.Settings.ViewModels;
using System.Windows;
using Wpf.Ui.Appearance;

namespace StreamBoard
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SettingsStorage>();
            services.AddSingleton<ServerConfigsStorage>();

            services.AddSingleton<StartupService>();
            services.AddSingleton<PrivilegeService>();

            services.AddSingleton<HttpServer>(sp =>
            {
                var storage = sp.GetRequiredService<ServerConfigsStorage>();
                var homeController = new HomeController(storage.Current.Http);

                var httpRouter = new HttpRouter([homeController]);

                return new HttpServer(storage.Current.Http, httpRouter);
            });

            services.AddSingleton<HttpServerViewModel>();
            services.AddSingleton<ActionRegistry>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<GridDeckViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var storage = ServiceProvider.GetRequiredService<SettingsStorage>();
            var privilegeService = ServiceProvider.GetRequiredService<PrivilegeService>();

            var httpServer = ServiceProvider.GetRequiredService<HttpServer>();
            var registry = ServiceProvider.GetRequiredService<ActionRegistry>();

            ApplicationThemeManager.Apply(
                storage.Current.Theme == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light
            );

            if (storage.Current.RunAsAdmin && !privilegeService.IsRunAsAdmin())
            {
                privilegeService.RestartAsAdmin();

                Application.Current.Shutdown();
                return;
            }

            if (httpServer != null && httpServer.ShouldAutoStart && !httpServer.IsRunning)
            {
                await httpServer.Start();
            }

            registry.RegisterActions();
        }
    }

}
