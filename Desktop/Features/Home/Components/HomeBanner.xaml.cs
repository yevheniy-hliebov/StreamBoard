using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Updater.Services;

namespace StreamTabula.Features.Home.Components
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
