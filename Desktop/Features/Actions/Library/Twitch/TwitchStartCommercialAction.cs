using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Actions.Models;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Integrations.Twitch.Services;
using StreamTabula.Controls.Icons;

namespace StreamTabula.Features.Actions.Library.Twitch;

[ActionDiscriminator("twitch_start_commercial")]
public class TwitchStartCommercialAction : TwitchBaseAction
{
    public static readonly ActionMetadata StaticMetadata = new(
        Name: "Start Commercial",
        DialogTitle: "Start Commercial Settings",
        Icon: FluentIconType.Bank
    );

    [JsonIgnore]
    public override ActionMetadata Metadata => StaticMetadata;

    private int _commercialLength = 30;

    [InputField("Length (Seconds)", Hint = "Valid: 30, 60, 90, 120, 150, 180")]
    [JsonPropertyName("commercial_length")]
    public int CommercialLength
    {
        get => _commercialLength;
        set
        {
            int val = value < 30 ? 30 : value;
            if (SetProperty(ref _commercialLength, val))
                OnPropertyChanged(nameof(Label));
        }
    }

    [JsonIgnore]
    public override string Label => $"{Metadata.Name} ({CommercialLength}s)";

    public override async Task ExecuteAsync(object? data = null)
    {
        try
        {
            var gateway = App.ServiceProvider.GetRequiredService<ITwitchAccountsGateway>();
            var broadcaster = gateway.Broadcaster;

            if (!broadcaster.Session.IsAuthenticated || broadcaster.Session.User?.Id == null)
            {
                return;
            }

            await broadcaster.Api.Production.StartCommercial(broadcaster.Session.User.Id, CommercialLength);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Twitch Start Commercial] Error: {ex.Message}");
        }
    }

    public override BaseAction Copy() => new TwitchStartCommercialAction
    {
        Id = this.Id,
        CommercialLength = this.CommercialLength
    };
}