using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Servers.Views.Pages;
using System.Windows;

namespace StreamTabula.Features.Servers.Services;

public class LocalServerBootstrapper
{
    public static async Task EnsureStartedAsync(IServiceProvider serviceProvider)
    {
        var localServer = serviceProvider.GetRequiredService<IMobileServer>();
        try
        {
            if (localServer?.ShouldAutoStart == true && !localServer.IsRunning)
            {
                await localServer.StartAsync();
            }
        }
        catch (InvalidOperationException ex)
        {
            var currentShutdownMode = Application.Current.ShutdownMode;
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var portConflictBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Port Already in Use",
                Content = $"{ex.Message}\n\nPlease change the server port in the settings to continue.",
                PrimaryButtonText = "Go to server settings",
                CloseButtonText = "Close",
                MaxWidth = 450,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var result = await portConflictBox.ShowDialogAsync();

            Application.Current.ShutdownMode = currentShutdownMode;

            if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                var navigationService = serviceProvider.GetRequiredService<Features.Navigation.Services.NavigationService>();
                navigationService.NavigateTo(typeof(LocalServerPage));
            }
        }
        catch (Exception ex)
        {
            var currentShutdownMode = Application.Current.ShutdownMode;
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var errorBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Server Startup Error",
                Content = ex.Message,
                CloseButtonText = "Close",
                MaxWidth = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            await errorBox.ShowDialogAsync();

            Application.Current.ShutdownMode = currentShutdownMode;
        }
    }
}