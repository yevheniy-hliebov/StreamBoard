using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Cards;
using StreamBoard.Components.Navigation;
using StreamBoard.Core.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Integrations.Common.Views.Pages
{
    public partial class IntegrationsPage : Page
    {
        public IntegrationsPage() => InitializeComponent();

        private void OnCardClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavigationCard card)
            {
                NavigateTo(card.Title);
            }
        }

        private void NavigateTo(string pageName)
        {
            var pageService = App.ServiceProvider.GetRequiredService<PageService>();
            var targetType = pageService.GetPageTypeByName(pageName);

            var mainWindow = Window.GetWindow(this) as MainWindow;
            var navComponent = mainWindow?.FindName("RootNavView") as MainNavView;

            navComponent?.GetNavigation().Navigate(targetType);
        }
    }
}
