using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_create_stream_marker")]
    public class TwitchCreateStreamMarkerAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Create Marker",
            DialogTitle: "Stream Marker Settings",
            Icon: FluentIconType.Bookmarks
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _description = string.Empty;

        [ActionSetting("Description", "Optional. Max 140 chars.")]
        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set
            {
                string safeValue = value?.Length > 140 ? value[..140] : (value ?? string.Empty);

                if (SetProperty(ref _description, safeValue))
                    OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Description)) return Metadata.Name;
                return $"{Metadata.Name} ({Description})";
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

                string? finalDescription = string.IsNullOrWhiteSpace(Description) ? null : Description;

                await broadcaster.Api.Production.CreateStreamMarker(
                    broadcaster.User.Id,
                    finalDescription
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Twitch Stream Marker] Error: {ex.Message}");
            }
        }

        public override DeckAction Copy() => new TwitchCreateStreamMarkerAction
        {
            Id = this.Id,
            Description = this.Description
        };
    }
}