using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;

namespace StreamTabula.Features.Actions.Library.System
{
    [ActionDiscriminator("lock_pc")]
    public class LockPCAction : SystemBaseAction
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

        public override Task ExecuteAsync(ActionExecutionContext context)
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

        public override BaseAction Copy() => new LockPCAction
        {
            Id = this.Id
        };
    }
}