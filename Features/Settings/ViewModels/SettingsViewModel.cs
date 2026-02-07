using StreamBoard.Core;
using StreamBoard.Features.Settings.Services;
using Wpf.Ui.Appearance;

namespace StreamBoard.Features.Settings.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly SettingsStorage _storage;

        public SettingsViewModel(SettingsStorage storage)
        {
            _storage = storage;
        }

        public bool IsDarkTheme
        {
            get => _storage.Current.Theme == "Dark";
            set
            {
                if (IsDarkTheme == value) return;
                _storage.Current.Theme = value ? "Dark" : "Light";
                _storage.Save();

                ApplicationThemeManager.Apply(value ? ApplicationTheme.Dark : ApplicationTheme.Light);
                OnPropertyChanged();
            }
        }
    }
}
