using StreamTabula.Features.Settings.ViewModels;
using System.Windows.Controls;

namespace StreamTabula.Features.Settings.Views.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsViewModel settingsVM)
    {
        InitializeComponent();

        DataContext = settingsVM;
    }
}
