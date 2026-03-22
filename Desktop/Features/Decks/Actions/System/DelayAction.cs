using System.Text.Json.Serialization;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;

namespace StreamBoard.Features.Decks.Actions.System
{
    [ActionDiscriminator("delay")]
    public class DelayAction : SystemDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Delay",
            DialogTitle: "Set Delay",
            Icon: FluentIconType.Timer
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private int _delayMs = 500;

        [ActionSetting("Delay (milliseconds)", "Enter delay in ms (e.g., 500)")]
        [JsonPropertyName("delay_ms")]
        public int DelayMs
        {
            get => _delayMs;
            set
            {
                _delayMs = value < 0 ? 0 : value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} for {DelayMs} ms";

        public override Task ExecuteAsync(object? data = null)
        {
            if (DelayMs > 0)
            {
                return Task.Delay(DelayMs);
            }

            return Task.CompletedTask;
        }

        public override DeckAction Copy() => new DelayAction
        {
            Id = this.Id,
            DelayMs = this.DelayMs
        };
    }
}