using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Integrations.OBS.ViewModels;

namespace StreamTabula.Features.Integrations.OBS.Views.Pages;

public partial class OBSSettingsPage : Page
{
    public OBSSettingsPage()
    {
        InitializeComponent();

        this.DataContext = App.ServiceProvider.GetRequiredService<OBSSettingsViewModel>();
    }
}
