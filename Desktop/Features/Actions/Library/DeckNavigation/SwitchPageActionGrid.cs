using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Actions.Attributes;
using StreamTabula.Features.Decks.Services;
using System.Text.Json.Serialization;

namespace StreamTabula.Features.Actions.Library.DeckNavigation;

[ActionDiscriminator("deck_navigation_grid_switch_page")]
[ActionInfo("Switch Page (Grid)", "Jump to a specific grid page", FluentIconType.Switch)]
public class SwitchPageActionGrid : DeckNavigationBaseAction
{
    [JsonPropertyName("target_page_id")]
    public string TargetPageId { get; set; } = string.Empty;

    [JsonIgnore]
    [DropdownField("Select Page", typeof(GridPageOptionsProvider), Hint = "Choose a page to jump to...")]
    public string PageSelection
    {
        get
        {
            if (string.IsNullOrEmpty(TargetPageId)) return string.Empty;

            var storage = App.ServiceProvider.GetRequiredService<GridDeckStorage>();
            var page = storage.Current.PagesState.AllPages.FirstOrDefault(p => p.Id == TargetPageId);

            return page != null ? $"{page.Name} [{page.Id}]" : $"Unknown Page [{TargetPageId}]";
        }
        set
        {
            if (string.IsNullOrEmpty(value)) return;

            var start = value.LastIndexOf('[');
            var end = value.LastIndexOf(']');

            if (start != -1 && end != -1 && end > start)
            {
                TargetPageId = value.Substring(start + 1, end - start - 1);
                OnPropertyChanged();
                OnPropertyChanged(nameof(Label));
            }
        }
    }

    [JsonIgnore]
    public override string Label
    {
        get
        {
            if (string.IsNullOrEmpty(TargetPageId)) return Metadata.Name;

            var storage = App.ServiceProvider.GetRequiredService<GridDeckStorage>();
            var page = storage.Current.PagesState.AllPages.FirstOrDefault(p => p.Id == TargetPageId);

            return page != null ? $"{Metadata.Name} ({page.Name})" : Metadata.Name;
        }
    }

    public override Task ExecuteAsync(object? data = null)
    {
        if (!string.IsNullOrWhiteSpace(TargetPageId))
        {
            GridDeckNavigationBus.RequestSwitchPage(TargetPageId);
        }
        return Task.CompletedTask;
    }
}