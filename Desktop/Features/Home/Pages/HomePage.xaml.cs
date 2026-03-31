using StreamBoard.Components.Cards;
using StreamBoard.Components.Navigation;
using StreamBoard.Features.Decks.Views.Pages;
using StreamBoard.Features.Integrations.Obs.Views.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Home.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void OnCardClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavigationCard card && card.Tag is string destination)
            {
                NavigateTo(destination);
            }
        }

        private void NavigateTo(string destination)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            var navComponent = mainWindow.FindName("RootNavView") as MainNavView;
            var navigationControl = navComponent?.GetNavigation();

            if (navigationControl == null) return;

            switch (destination)
            {
                case "GridDeck":
                    navigationControl.Navigate(typeof(GridDeckPage));
                    break;
                case "KeyboardDeck":
                    // navigationControl.Navigate(new KeyboardDeckPage());
                    break;
                case "OBS":
                    navigationControl.Navigate(typeof(ObsSettingsPage));
                    break;
                case "Twitch":
                    // navigationControl.Navigate(new TwitchSettingsPage());
                    break;
            }
        }
    }
}