using StreamTabula.Features.Navigation.Views.Components;
using StreamTabula.Features.Updater.Models;
using StreamTabula.Features.Updater.Services;

namespace StreamTabula.Features.Home.Views.Pages;

public partial class HomePage : NavigationHubPage
{
    public AppInfoModel AppInfo { get; }

    public HomePage(AppInfoService appInfoService)
    {
        InitializeComponent();

        AppInfo = appInfoService.AppInfo;

        DataContext = this;
    }
}