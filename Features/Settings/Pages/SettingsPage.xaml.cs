using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Settings.ViewModels;
using System.Windows.Controls;

namespace StreamBoard.Features.Settings.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
        }
    }
}
