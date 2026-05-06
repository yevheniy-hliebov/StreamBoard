using Microsoft.Win32;
using System.Diagnostics;

namespace StreamTabula.Features.Settings.Services
{
    public class StartupService
    {
        private const string AppName = "StreamTabula";

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
