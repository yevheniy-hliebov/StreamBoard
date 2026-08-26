using System.Diagnostics;
using System.Runtime.InteropServices;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("lock_pc")]
[ActionInfo("Lock PC", "Lock Workstation", FluentIconType.Lock)]
public class LockPCAction : SystemBaseAction
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool LockWorkStation();

    public override Task ExecuteAsync(object? data = null)
    {
        try
        {
            LockWorkStation();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not lock PC: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}