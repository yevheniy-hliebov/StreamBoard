using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_create_clip")]
    public class TwitchCreateClipAction : TwitchBaseAction
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

        [InputField("Title", Hint = "Optional. Title for the clip.")]
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

        [InputField("Duration", Hint = "Optional. Duration in seconds (0 = default).")]
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

        public override async Task ExecuteAsync(ActionExecutionContext context)
        {
            try
            {
                string resolvedClipTitle = ResolveVariable(ClipTitle, context);

                if (string.IsNullOrWhiteSpace(resolvedClipTitle)) return;

                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (!broadcaster.IsAuth || broadcaster.User?.Id == null || broadcaster.Api == null)
                {
                    return;
                }

                string? finalTitle = string.IsNullOrWhiteSpace(resolvedClipTitle) ? null : resolvedClipTitle;
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

        public override BaseAction Copy() => new TwitchCreateClipAction
        {
            Id = this.Id,
            ClipTitle = this.ClipTitle,
            DurationSeconds = this.DurationSeconds
        };
    }
}