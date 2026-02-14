using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Servers.Controllers;
using StreamBoard.Features.Servers.Services;
using StreamBoard.Features.Servers.ViewModels;
using StreamBoard.Features.Settings.Services;
using StreamBoard.Features.Settings.ViewModels;
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

                return new HttpServer(storage.Current.Http, [ homeController ]);
            });

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<HttpServerViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var storage = ServiceProvider.GetRequiredService<SettingsStorage>();
            var privilegeService = ServiceProvider.GetRequiredService<PrivilegeService>();

            var httpServer = ServiceProvider.GetRequiredService<HttpServer>();

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
                await httpServer.StartAsync();
            }
        }
    }

}
