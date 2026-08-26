using System.Diagnostics;
using System.Runtime.InteropServices;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("close_active_window")]
[ActionInfo("Close Active Window", "Close Active Window", FluentIconType.ChromeClose)]
public class CloseActiveWindowAction : SystemBaseAction
{
    private const uint WM_CLOSE = 0x0010;

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public override Task ExecuteAsync(object? data = null)
    {
        try
        {
            IntPtr activeWindow = GetForegroundWindow();

            if (activeWindow != IntPtr.Zero)
            {
                GetWindowThreadProcessId(activeWindow, out uint activeProcessId);

                uint currentProcessId = (uint)Process.GetCurrentProcess().Id;

                if (activeProcessId != currentProcessId)
                {
                    PostMessage(activeWindow, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    Debug.WriteLine("Ignored: StreamTabula is the active window, preventing self-close.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not close active window: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}