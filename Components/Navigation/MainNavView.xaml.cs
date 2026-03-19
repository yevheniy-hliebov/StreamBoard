using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Home.Pages;
using StreamBoard.Features.Settings.Pages;
using StreamBoard.Features.Servers.Pages;
using StreamBoard.Features.Decks.Views.Pages; 
using StreamBoard.Features.Settings.Services;
using System;
using System.Windows;
using System.Windows.Controls;

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
            var startupPage = settings.Current.StartupPage;

            Type pageToNavigate = startupPage switch
            {
                "Grid Deck" => typeof(GridDeckPage),
                "HTTP Server" => typeof(HttpServerPage),
                "Settings" => typeof(SettingsPage),
                _ => typeof(HomePage)
            };

            // Виконуємо перехід
            RootNavigation.Navigate(pageToNavigate);
        }
    }
}