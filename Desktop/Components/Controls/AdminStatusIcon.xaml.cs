using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Settings.Services;
using System.Windows;
using System.Windows.Controls;

namespace StreamTabula.Components.Controls
{
    public partial class AdminStatusIcon : UserControl
    {
        public AdminStatusIcon()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            var privilegeService = App.ServiceProvider.GetRequiredService<PrivilegeService>();

            if (privilegeService != null)
            {
                bool isAdmin = privilegeService.IsRunAsAdmin();
                if (isAdmin)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}