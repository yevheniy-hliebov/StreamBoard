namespace StreamBoard.Features.Settings.Models
{
    public class SettingsModel
    {       
        public bool MinimizeToTray { get; set; } = false;
        public bool StartMinimized { get; set; } = false;
        public bool StartupWithWindows { get; set; } = false;
        public bool RunAsAdmin { get; set; } = false;
        public string Theme { get; set; } = "Dark";

    }
}
