using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Home.Pages;
using StreamBoard.Features.Settings.Pages;
using StreamBoard.Features.Servers.Pages;
using StreamBoard.Features.Decks.Views.Pages;
using StreamBoard.Features.Settings.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using StreamBoard.Features.Integrations.Obs.Views.Pages;
using StreamBoard.Features.Integrations.Twitch.Views.Pages;
using StreamBoard.Core.Services;

namespace StreamBoard.Components.Navigation
{
    public partial class MainNavView : UserControl
    {
        public Wpf.Ui.Controls.NavigationView GetNavigation() => RootNavigation;

        public MainNavView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            var settings = App.ServiceProvider.GetRequiredService<SettingsStorage>();
            var pageService = App.ServiceProvider.GetRequiredService<PageService>();

            var startupPage = settings.Current.StartupPage;
            Type pageToNavigate = pageService.GetPageTypeByName(startupPage);

            RootNavigation.Navigate(pageToNavigate);
        }
    }
}