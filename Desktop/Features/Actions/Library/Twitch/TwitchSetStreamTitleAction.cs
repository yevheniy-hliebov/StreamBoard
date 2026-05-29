using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Integrations.Twitch.Services;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_set_steam_title")]
    public class TwitchSetStreamTitleAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Set Stream Title",
            DialogTitle: "Set Stream Title Settings",
            Icon: FluentIconType.Edit
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _title = string.Empty;

        [InputField("Title", Hint = "Enter title...")]
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

        public override async Task ExecuteAsync(ActionExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(Title)) return;
            string resolvedTitle = ResolveVariable(Title, context);
            if (string.IsNullOrWhiteSpace(resolvedTitle)) return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (broadcaster.IsAuth && broadcaster.User != null)
                {
                    string? broadcasterId = gateway.Broadcaster.User?.Id;

                    if (broadcasterId != null && broadcaster.Api != null)
                    {
                        await broadcaster.Api.Channel.UpdateTitle(broadcasterId, resolvedTitle);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch set stream title error: {ex.Message}");
                throw;
            }
        }

        public override BaseAction Copy() => new TwitchSetStreamTitleAction
        {
            Id = this.Id,
            Title = this.Title
        };
    }
}