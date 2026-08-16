using Microsoft.Win32;
using System.Diagnostics;

namespace StreamTabula.Features.Settings.Helpers;

public static class WindowsStartupHelper
{
    /// <summary>
    /// Enables or disables the application from launching when Windows boots (via the registry).
    /// </summary>
    public static async Task SetStartWithWindowsAsync(string AppName, bool enable)
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName
                ?? AppDomain.CurrentDomain.BaseDirectory + AppName + ".exe";

            using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key != null)
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
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Startup Configuration Error",
                Content = $"Failed to configure Windows startup settings.\n\nDetails:\n{ex.Message}",
                CloseButtonText = "OK"
            };

            await messageBox.ShowDialogAsync();
        }
    }
}
