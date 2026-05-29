using System.Text.Json.Serialization;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Helpers;

namespace StreamTabula.Features.Actions.Library.System
{
    [ActionDiscriminator("delay")]
    public class DelayAction : SystemBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Delay",
            DialogTitle: "Set Delay",
            Icon: FluentIconType.Timer
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private int _delayMs = 500;
        private bool _playTickEverySecond = false;
        private int _soundVolume = 50;

        [InputField("Delay (milliseconds)", Hint = "Enter delay in ms (e.g., 1000)")]
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

        [InputField("Play Tick Sound", Hint = "Play a sound every second during the delay")]
        [JsonPropertyName("play_tick_every_second")]
        public bool PlayTickEverySecond
        {
            get => _playTickEverySecond;
            set => SetProperty(ref _playTickEverySecond, value);
        }

        [InputField("Sound Volume", Hint = "Volume for the tick sound (0-100)")]
        [JsonPropertyName("sound_volume")]
        public int SoundVolume
        {
            get => _soundVolume;
            set => SetProperty(ref _soundVolume, value);
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} for {DelayMs} ms";

        public override async Task ExecuteAsync(ActionExecutionContext context)
        {
            if (DelayMs <= 0) return;

            if (!PlayTickEverySecond || DelayMs < 1000)
            {
                await Task.Delay(DelayMs);
            }
            else
            {
                int remainingMs = DelayMs;
                while (remainingMs > 0)
                {
                    AudioPlayerService.Play("Assets/Sounds/tick.mp3", SoundVolume);

                    int sleepTime = Math.Min(remainingMs, 1000);
                    await Task.Delay(sleepTime);
                    remainingMs -= sleepTime;
                }
            }
        }

        public override BaseAction Copy() => new DelayAction
        {
            Id = this.Id,
            DelayMs = this.DelayMs,
            PlayTickEverySecond = this.PlayTickEverySecond,
            SoundVolume = this.SoundVolume
        };
    }
}