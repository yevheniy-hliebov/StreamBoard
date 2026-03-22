using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("lock_pc")]
    public class LockPCAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Lock PC",
            DialogTitle: "Lock Workstation",
            Icon: FluentIconType.Lock
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        [JsonIgnore]
        public override string Label => Metadata.Name;

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

        public override DeckAction Copy() => new LockPCAction
        {
            Id = this.Id
        };
    }
}