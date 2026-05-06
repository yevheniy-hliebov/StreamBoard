using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Components.Cards;
using StreamTabula.Features.Navigation.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace StreamTabula.Features.Navigation.Views.Components
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
