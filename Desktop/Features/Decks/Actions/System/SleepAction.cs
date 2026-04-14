using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("sleep_pc")]
    public class SleepAction : SystemDeckAction
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

        public override DeckAction Copy() => new SleepAction
        {
            Id = this.Id
        };
    }
}