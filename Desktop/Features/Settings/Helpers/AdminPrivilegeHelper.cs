using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace StreamTabula.Features.Settings.Helpers;

public static class AdminPrivilegeHelper
{
    /// <summary>
    /// Checks whether the current process is running with administrator privileges.
    /// </summary>
    public static bool IsRunningAsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);

        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Requests administrator privileges (via UAC) and restarts the application.
    /// </summary>
    public static void RestartAsAdministrator()
    {
        var exePath = Environment.ProcessPath;

        if (string.IsNullOrEmpty(exePath)) return;

        ProcessStartInfo startInfo = new()
        {
            FileName = exePath,
            UseShellExecute = true,
            Verb = "runas"
        };

        try
        {
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }
        catch
        {
            Debug.WriteLine("User cancelled elevation request.");
        }
    }
}