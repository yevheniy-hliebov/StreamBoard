using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Servers.Pages;
using StreamTabula.Features.Settings.Services;
using System.Windows;

namespace StreamTabula.Features.Servers.Services
{
    public class LocalServerBootstrapper
    {
        public static async Task EnsureStartedAsync(IServiceProvider serviceProvider)
        {
            var LocalServer = serviceProvider.GetRequiredService<LocalServer>();
            try
            {
                if (LocalServer != null && LocalServer.ShouldAutoStart && !LocalServer.IsRunning)
                {
                    await LocalServer.Start();
                }
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    var currentShutdownMode = Application.Current.ShutdownMode;
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    var serverConfigsStorage = serviceProvider.GetRequiredService<ServerConfigsStorage>();
                    string targetIp = serverConfigsStorage.Current.Local.Address ?? "localhost";

                    var authBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Administrator Privileges Required",
                        Content = $"Elevated privileges are required to start the server on {targetIp}. What would you like to do?",
                        PrimaryButtonText = "Run as Admin",
                        SecondaryButtonText = "Always run as Admin",
                        CloseButtonText = "Go to server settings",
                        MaxWidth = 550,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    var result = await authBox.ShowDialogAsync();

                    Application.Current.ShutdownMode = currentShutdownMode;

                    var settingStorage = serviceProvider.GetRequiredService<SettingsStorage>();
                    var privilegeService = serviceProvider.GetRequiredService<PrivilegeService>();

                    if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                    {
                        privilegeService.RestartAsAdmin();
                        Application.Current.Shutdown();
                        return;
                    }
                    else if (result == Wpf.Ui.Controls.MessageBoxResult.Secondary)
                    {
                        settingStorage.Current.RunAsAdmin = true;
                        settingStorage.Save();

                        privilegeService.RestartAsAdmin();
                        Application.Current.Shutdown();
                        return;
                    }
                    else
                    {
                        var navigationService = serviceProvider.GetRequiredService<Features.Navigation.Services.NavigationService>();
                        navigationService.NavigateTo(typeof(LocalServerPage));
                    }
                }
                else
                {
                    var currentShutdownMode = Application.Current.ShutdownMode;
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    var errorBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Server Error",
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
    }
}
