using System.Windows.Controls;
using StreamTabula.Features.Integrations.Twitch.ViewModels;

namespace StreamTabula.Features.Integrations.Twitch.Views.Pages;

public partial class TwitchSettingsPage : Page
{
    public TwitchSettingsPage(TwitchSettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}