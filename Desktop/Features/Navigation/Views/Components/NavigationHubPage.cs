using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Controls.Cards;
using StreamTabula.Features.Navigation.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Navigation.Views.Components
{
    public abstract class NavigationHubPage : Page
    {
        protected readonly NavigationService NavService;

        protected NavigationHubPage(NavigationService navService)
        {
            NavService = navService;
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
