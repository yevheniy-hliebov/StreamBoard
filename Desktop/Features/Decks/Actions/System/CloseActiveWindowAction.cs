using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("close_active_window")]
    public class CloseActiveWindowAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Close Active Window",
            DialogTitle: "Close Active Window",
            Icon: FluentIconType.ChromeClose
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        [JsonIgnore]
        public override string Label => Metadata.Name;

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
                        Debug.WriteLine("Ignored: StreamBoard is the active window, preventing self-close.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not close active window: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public override DeckAction Copy() => new CloseActiveWindowAction
        {
            Id = this.Id
        };
    }
}