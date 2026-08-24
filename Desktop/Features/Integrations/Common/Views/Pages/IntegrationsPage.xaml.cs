using StreamTabula.Features.Navigation.Services;
using StreamTabula.Features.Navigation.Views.Components;

namespace StreamTabula.Features.Integrations.Common.Views.Pages
{
    public partial class IntegrationsPage : NavigationHubPage
    {
        public IntegrationsPage(NavigationService navService) : base(navService)
        {
            InitializeComponent();
        }
    }
}
