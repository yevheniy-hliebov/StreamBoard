using Microsoft.Win32;
using System.Diagnostics;

namespace StreamBoard.Features.Settings.Services
{
    public class StartupService
    {
        private const string AppName = "StreamBoard";

        public void SetStartup(bool enable)
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName
                    ?? AppDomain.CurrentDomain.BaseDirectory + AppName + ".exe";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!)
                {
                    if (enable)
                    {
                        key.SetValue(AppName, $"\"{exePath}\"");
                    }
                    else
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[StartupService] Error: {ex.Message}");
            }
        }
    }
}
