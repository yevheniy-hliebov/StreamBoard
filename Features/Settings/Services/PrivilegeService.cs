using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace StreamBoard.Features.Settings.Services
{
    public class PrivilegeService
    {
        public bool IsRunAsAdmin()
        {
            using(WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public void RestartAsAdmin()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (string.IsNullOrEmpty(exePath)) return;

            ProcessStartInfo startInfo = new ProcessStartInfo
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
}
