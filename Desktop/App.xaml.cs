using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Bootstrapping;
using System.Diagnostics;
using System.Windows;

namespace StreamTabula;

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

        try
        {
            await AppInitializer.InitializeAsync(ServiceProvider);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[CRITICAL] Startup failed: {ex}");

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Fatal startup error",
                Content = $"Failed to start the application.\n\nDetails:\n{ex.Message}",
                CloseButtonText = "OK"
            };

            await messageBox.ShowDialogAsync();

            Current.Shutdown();
        }
        
    }
}