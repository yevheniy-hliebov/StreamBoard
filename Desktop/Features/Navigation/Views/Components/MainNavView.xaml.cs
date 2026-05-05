using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Components.Controls;
using StreamTabula.Features.Integrations.Common.Views.Components;
using StreamTabula.Features.Navigation.Services;
using StreamTabula.Features.Settings.Services;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace StreamTabula.Features.Navigation.Views.Components
{
    public partial class MainNavView : UserControl
    {
        public NavigationView GetNavigation() => RootNavigation;

        public MainNavView()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            BuildNavigationMenu();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            var settings = App.ServiceProvider.GetRequiredService<SettingsStorage>();
            var pageService = App.ServiceProvider.GetRequiredService<NavigationService>();

            pageService.RegisterNavigationControl(RootNavigation);

            var startupPage = settings.Current.StartupPage;
            pageService.NavigateTo(startupPage);
        }

        private void BuildNavigationMenu()
        {
            var pageService = App.ServiceProvider.GetRequiredService<NavigationService>();

            var parentMenus = new Dictionary<string, NavigationViewItem>();

            foreach (var page in pageService.AllPages)
            {
                var navItem = new NavigationViewItem
                {
                    Content = page.Name,
                    TargetPageType = page.PageType
                };

                if (page.FluentIcon != null)
                {
                    navItem.Icon = new FluentIcon { IconType = page.FluentIcon.Value, FontSize = 16 };
                }
                else if (page.IntegrationIcon != null)
                {
                    navItem.Icon = new IntegrationIcon { IconType = page.IntegrationIcon.Value, Size = 16 };
                }

                if (page.IsFooter)
                {
                    if (RootNavigation.FooterMenuItems.Count == 0)
                        RootNavigation.FooterMenuItems.Add(new NavigationViewItemSeparator());

                    RootNavigation.FooterMenuItems.Add(navItem);
                }
                else if (page.ParentName != null && parentMenus.TryGetValue(page.ParentName, out var parentItem))
                {
                    parentItem.MenuItems.Add(navItem);
                }
                else
                {
                    RootNavigation.MenuItems.Add(navItem);
                    parentMenus[page.Name] = navItem;
                }

                if (page.AddSeparatorAfter)
                {
                    RootNavigation.MenuItems.Add(new NavigationViewItemSeparator());
                }
            }
        }
    }
}