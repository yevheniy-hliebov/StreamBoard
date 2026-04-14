using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_set_steam_title")]
    public class TwitchSetStreamTitleAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Set Stream Title",
            DialogTitle: "Set Stream Title Settings",
            Icon: FluentIconType.Edit
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _title = string.Empty;

        [ActionSetting("Title", "Enter title...")]
        [JsonPropertyName("title")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return Metadata.Name;
                }
                return $"{Metadata.Name} ({Title})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(Title)) return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (broadcaster.IsAuth && broadcaster.User != null)
                {
                    string? broadcasterId = gateway.Broadcaster.User?.Id;

                    if (broadcasterId != null && broadcaster.Api != null)
                    {
                        await broadcaster.Api.Channel.UpdateTitle(broadcasterId, Title);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch set stream title error: {ex.Message}");
                throw;
            }
        }

        public override DeckAction Copy() => new TwitchSetStreamTitleAction
        {
            Id = this.Id,
            Title = this.Title
        };
    }
}