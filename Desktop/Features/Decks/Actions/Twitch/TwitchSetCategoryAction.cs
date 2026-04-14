using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Core.Models;
using StreamBoard.Features.Decks.Attributes;
using StreamBoard.Features.Decks.Models;
using StreamBoard.Features.Integrations.Twitch.Services;

namespace StreamBoard.Features.Decks.Actions.Twitch
{
    [ActionDiscriminator("twitch_set_category")]
    public class TwitchSetCategoryAction : TwitchDeckAction
    {
        public static readonly ActionMetadata StaticMetadata = new(
            Name: "Set Category",
            DialogTitle: "Set Stream Category",
            Icon: FluentIconType.Game
        );

        [JsonIgnore]
        public override ActionMetadata Metadata => StaticMetadata;

        private string _categoryId = string.Empty;
        private string _categoryName = string.Empty;

        [ActionSetting("Category", "Type to search...", searchProvider: typeof(TwitchCategorySearchProvider), displayProperty: "Category")]
        [JsonPropertyName("category_id")]
        public string CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        [JsonPropertyName("category_name")]
        public string Category
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }

        [JsonIgnore]
        public override string Label
        {
            get
            {
                if (string.IsNullOrEmpty(Category)) return Metadata.Name;
                return $"{Metadata.Name} ({Category})";
            }
        }

        public override async Task ExecuteAsync(object? data = null)
        {
            if (string.IsNullOrWhiteSpace(CategoryId)) return;

            try
            {
                var gateway = App.ServiceProvider.GetRequiredService<TwitchAccountsGateway>();
                var broadcaster = gateway.Broadcaster;

                if (broadcaster.IsAuth && broadcaster.User?.Id != null && broadcaster.Api != null)
                {
                    await broadcaster.Api.Channel.SetCategory(broadcaster.User.Id, CategoryId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Twitch set category error: {ex.Message}");
            }
        }

        public override DeckAction Copy() => new TwitchSetCategoryAction
        {
            Id = this.Id,
            CategoryId = this.CategoryId,
            Category = this.Category
        };
    }
}