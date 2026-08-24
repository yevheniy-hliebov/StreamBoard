using StreamTabula.Features.Decks.Views.Pages;
using StreamTabula.Features.Integrations.Common.Views.Pages;
using StreamTabula.Features.Integrations.OBS.Views.Pages;
using StreamTabula.Features.Integrations.Twitch.Views.Pages;
using StreamTabula.Features.Navigation.Models;
using StreamTabula.Controls.Icons;
using StreamTabula.Features.Home.Views.Pages;
using StreamTabula.Features.Settings.Views.Pages;
using StreamTabula.Features.Servers.Views.Pages;

namespace StreamTabula.Features.Navigation.Services;

public class NavigationService
{
    private Wpf.Ui.Controls.NavigationView? _navigationControl;
    private Type? _pendingNavigationType;

    public IReadOnlyList<AppRoute> AllPages { get; } = [
        new("Home", typeof(HomePage), FluentIcon: FluentIconType.Home, AddSeparatorAfter: true),

        new("Grid Deck", typeof(GridDeckPage), FluentIcon: FluentIconType.Grid, AddSeparatorAfter: true),

        new("Integrations", typeof(IntegrationsPage), FluentIcon: FluentIconType.Puzzle),
        new("OBS Studio", typeof(OBSSettingsPage), IntegrationIcon: IntegrationIconType.Obs, ParentName: "Integrations"),
        new("Twitch", typeof(TwitchSettingsPage), IntegrationIcon: IntegrationIconType.Twitch, ParentName: "Integrations", AddSeparatorAfter: true),

        new("Local Server", typeof(LocalServerPage), FluentIcon: FluentIconType.Network),

        new("Settings", typeof(SettingsPage), FluentIcon: FluentIconType.Settings, IsFooter: true)
    ];

    public bool RegisterNavigationControl(Wpf.Ui.Controls.NavigationView navControl)
    {
        _navigationControl = navControl;

        if (_pendingNavigationType != null)
        {
            NavigateTo(_pendingNavigationType);
            _pendingNavigationType = null;
            return true;
        }

        return false;
    }

    public Type GetPageTypeByName(string name)
    {
        return AllPages.FirstOrDefault(p => p.Name == name)?.PageType ?? typeof(HomePage);
    }

    public void NavigateTo(string pageName)
    {
        if (_navigationControl == null) return;

        var targetType = GetPageTypeByName(pageName);
        _navigationControl.Navigate(targetType);
    }

    public void NavigateTo(Type targetType)
    {
        if (_navigationControl == null)
        {
            _pendingNavigationType = targetType;
            return;
        }
        _navigationControl.Navigate(targetType);
    }
}