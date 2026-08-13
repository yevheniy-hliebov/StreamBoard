using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.System;

[ActionDiscriminator("sleep_pc")]
public class SleepAction : SystemBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Sleep",
        DialogTitle: "Put PC to Sleep",
        Icon: FluentIconType.Moon
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

    [JsonIgnore]
    public override string Label => Metadata.Name;

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

    public override BaseAction Copy() => new SleepAction
    {
        Id = this.Id
    };
}