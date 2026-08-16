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
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            ShowCrashDump(e.ExceptionObject as Exception, "AppDomain Unhandled");
        };

        DispatcherUnhandledException += (s, e) =>
        {
            ShowCrashDump(e.Exception, "Dispatcher Unhandled");
            e.Handled = true;
        };

        try
        {
            var services = new ServiceCollection();
            services.AddApplicationServices();
            ServiceProvider = services.BuildServiceProvider();
        }
        catch (Exception ex)
        {
            ShowCrashDump(ex, "DI Container Build Failed");
        }
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
    private void ShowCrashDump(Exception? ex, string stage)
    {
        Debug.WriteLine($"[CRITICAL] {stage}: {ex}");

        var messageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = "Fatal error",
            Content = $"A fatal error occurred ({stage}).\n\n{ex?.Message}\n\n{ex?.InnerException?.Message}",
            CloseButtonText = "OK",
        };

        messageBox.ShowDialogAsync();

        Environment.Exit(1);
    }
}