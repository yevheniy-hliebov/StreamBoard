using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.AppStartup;
using StreamTabula.Core.DI;
using System.Windows;

namespace StreamTabula
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices();
            ServiceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await AppInitializer.InitializeAsync(ServiceProvider);
        }
    }
}