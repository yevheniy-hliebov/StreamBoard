using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_create_clip")]
    public class TwitchCreateClipAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Create Clip",
            DialogTitle: "Create Clip Settings",
            Icon: FluentIconType.Video
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _clipTitle = string.Empty;
        private int _durationSeconds = 0;

        [ActionSetting("Title", "Optional. Title for the clip.")]
        [JsonPropertyName("clip_title")]
        public string ClipTitle
        {
            get => _clipTitle;
            set
            {
                if (SetProperty(ref _clipTitle, value))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [ActionSetting("Duration", "Optional. Duration in seconds (0 = default).")]
        [JsonPropertyName("duration_seconds")]
        public int DurationSeconds
        {
            get => _durationSeconds;
            set
            {
                int val = value < 0 ? 0 : value;
                SetProperty(ref _durationSeconds, val);
            }
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ClipTitle)) return Metadata.Name;
                return $"{Metadata.Name} ({ClipTitle})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (!broadcaster.IsAuth || broadcaster.User?.Id == null || broadcaster.Api == null)
                {
                    return;
                }

                string? finalTitle = string.IsNullOrWhiteSpace(ClipTitle) ? null : ClipTitle;
                float? finalDuration = DurationSeconds > 0 ? DurationSeconds : null;

                await broadcaster.Api.Production.CreateClip(
                    broadcaster.User.Id,
                    finalTitle,
                    finalDuration
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Twitch Create Clip] Error: {ex.Message}");
            }
        }

        public override DeckAction Copy() => new TwitchCreateClipAction
        {
            Id = this.Id,
            ClipTitle = this.ClipTitle,
            DurationSeconds = this.DurationSeconds
        };
    }
}