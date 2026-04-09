using System.Text.Json.Serialization;
using StreamBoard.Components.Controls;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Helpers;

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
        private bool _playTickEverySecond = false;
        private int _soundVolume = 50;

        [ActionSetting("Delay (milliseconds)", "Enter delay in ms (e.g., 1000)")]
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

        [ActionSetting("Play Tick Sound", "Play a sound every second during the delay")]
        [JsonPropertyName("play_tick_every_second")]
        public bool PlayTickEverySecond
        {
            get => _playTickEverySecond;
            set => SetProperty(ref _playTickEverySecond, value);
        }

        [ActionSetting("Sound Volume", "Volume for the tick sound (0-100)")]
        [JsonPropertyName("sound_volume")]
        public int SoundVolume
        {
            get => _soundVolume;
            set => SetProperty(ref _soundVolume, value);
        }

        [JsonIgnore]
        public override string Label => $"{Metadata.Name} for {DelayMs} ms";

        public override async Task ExecuteAsync(object? data = null)
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
                    // AudioPlayerService.Play("C:/Users/glebov/develop/projects/streamboard/Desktop/Assets/Sounds/tick.mp3", SoundVolume);

                    int sleepTime = Math.Min(remainingMs, 1000);
                    await Task.Delay(sleepTime);
                    remainingMs -= sleepTime;
                }
            }
        }

        public override DeckAction Copy() => new DelayAction
        {
            Id = this.Id,
            DelayMs = this.DelayMs,
            PlayTickEverySecond = this.PlayTickEverySecond,
            SoundVolume = this.SoundVolume
        };
    }
}