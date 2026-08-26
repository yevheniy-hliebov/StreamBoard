using System.Diagnostics;
using System.Runtime.InteropServices;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("sleep_pc")]
[ActionInfo("Sleep", "Put PC to Sleep", FluentIconType.Moon)]
public class SleepAction : SystemBaseAction
{
    [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

    public override Task ExecuteAsync(object? data = null)
    {
        try
        {
            SetSuspendState(false, false, false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not put PC to sleep: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}