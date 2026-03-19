using System.Text.Json.Serialization;

namespace StreamBoard.Features.Settings.Models
{
    public class SettingsModel
    {
        [JsonPropertyName("startup_page")]
        public string StartupPage { get; set; } = "Home";
        
        [JsonPropertyName("minimize_to_tray")]
        public bool MinimizeToTray { get; set; } = false;
        
        [JsonPropertyName("start_minimized")]
        public bool StartMinimized { get; set; } = false;
        
        [JsonPropertyName("startup_with_windows")]
        public bool StartupWithWindows { get; set; } = false;
        
        [JsonPropertyName("run_as_admin")]
        public bool RunAsAdmin { get; set; } = false;

        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "Dark";
    }
}
