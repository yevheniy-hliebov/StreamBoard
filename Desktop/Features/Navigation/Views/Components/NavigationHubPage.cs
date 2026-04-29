using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Components.Cards;
using StreamBoard.Features.Navigation.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamBoard.Features.Navigation.Views.Components
{
    public abstract class NavigationHubPage : Page
    {
        protected readonly NavigationService NavService;

        protected NavigationHubPage()
        {
            NavService = App.ServiceProvider.GetRequiredService<NavigationService>();
        }

        protected void OnCardClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is NavigationCard card)
            {
                NavService.NavigateTo(card.Title);
            }
        }
    }
}
