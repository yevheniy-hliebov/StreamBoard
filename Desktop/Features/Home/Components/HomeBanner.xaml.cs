using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamBoard.Features.Updater.Services;

namespace StreamBoard.Features.Home.Components
{
    public partial class HomeBanner : UserControl
    {
        public HomeBanner()
        {
            InitializeComponent();
            var appInfoService = App.ServiceProvider.GetRequiredService<AppInfoService>();

            this.DataContext = appInfoService.AppInfo;
        }
    }
}
