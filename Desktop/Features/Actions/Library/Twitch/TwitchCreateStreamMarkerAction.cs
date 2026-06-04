using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Core.Models;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;

namespace StreamTabula.Features.Actions.Library.Twitch
{
    [ActionDiscriminator("twitch_create_stream_marker")]
    public class TwitchCreateStreamMarkerAction : TwitchBaseAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Create Marker",
            DialogTitle: "Stream Marker Settings",
            Icon: FluentIconType.Bookmarks
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _description = string.Empty;

        [InputField("Description", Hint = "Optional. Max 140 chars.")]
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

        public override async Task ExecuteAsync(ActionExecutionContext context)
        {
            context.RuntimeVariables["twitchMarkerSuccess"] = "false";
            context.RuntimeVariables["twitchMarkerError"] = "";
            context.RuntimeVariables["twitchMarkerId"] = "";
            context.RuntimeVariables["twitchMarkerCreatedAt"] = "";
            context.RuntimeVariables["twitchMarkerPositionSeconds"] = "";

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (!broadcaster.IsAuth || broadcaster.User?.Id == null || broadcaster.Api == null)
                {
                    context.RuntimeVariables["twitchMarkerError"] = "Broadcaster account is not authenticated or API is unavailable.";
                    return;
                }

                string resolvedDescription = ResolveVariable(Description, context);
                string? finalDescription = string.IsNullOrWhiteSpace(resolvedDescription) ? null : resolvedDescription;

                var markerResponse = await broadcaster.Api.Production.CreateStreamMarker(
                    broadcaster.User.Id,
                    finalDescription
                );

                if (markerResponse != null)
                {
                    context.RuntimeVariables["twitchMarkerSuccess"] = "true";
                    context.RuntimeVariables["twitchMarkerId"] = markerResponse.Id;
                    context.RuntimeVariables["twitchMarkerCreatedAt"] = markerResponse.CreatedAt;
                    context.RuntimeVariables["twitchMarkerPositionSeconds"] = markerResponse.PositionSeconds.ToString();
                }
                else
                {
                    context.RuntimeVariables["twitchMarkerError"] = "Received empty response from Twitch API.";
                }
            }
            catch (Exception ex)
            {
                context.RuntimeVariables["twitchMarkerError"] = ex.Message;
            }
        }

        public override BaseAction Copy() => new TwitchCreateStreamMarkerAction
        {
            Id = this.Id,
            Description = this.Description
        };
    }
}