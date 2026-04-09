using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.AppStartup;
using StreamBoard.Core.DI;
using System.Windows;

namespace StreamBoard
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
