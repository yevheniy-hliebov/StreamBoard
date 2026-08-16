namespace StreamTabula.Features.Settings.Services;

public partial class SettingsStorage
{
    private void MigrateV1ToV2()
    {
        if (Current.OldProperties != null)
        {
            // Startup
            if (Current.OldProperties.TryGetValue("startup_page", out var sp)) Current.Startup.StartupPage = sp.GetString() ?? "Home";
            if (Current.OldProperties.TryGetValue("minimize_to_tray", out var mtt)) Current.Startup.MinimizeToTray = mtt.GetBoolean();
            if (Current.OldProperties.TryGetValue("start_minimized", out var sm)) Current.Startup.StartMinimized = sm.GetBoolean();
            if (Current.OldProperties.TryGetValue("startup_with_windows", out var sww)) Current.Startup.StartupWithWindows = sww.GetBoolean();
            if (Current.OldProperties.TryGetValue("run_as_admin", out var raa)) Current.Startup.RunAsAdmin = raa.GetBoolean();

            // Appearance
            if (Current.OldProperties.TryGetValue("theme", out var th)) Current.Appearance.Theme = th.GetString() ?? "Dark";

            // Updates
            if (Current.OldProperties.TryGetValue("update_channel", out var uc)) Current.Updates.UpdateChannel = uc.GetString() ?? "Stable releases";
            if (Current.OldProperties.TryGetValue("skipped_version", out var sv)) Current.Updates.SkippedVersion = sv.GetString();

            // Window
            if (Current.OldProperties.TryGetValue("window_width", out var ww)) Current.Window.Width = ww.GetDouble();
            if (Current.OldProperties.TryGetValue("window_height", out var wh)) Current.Window.Height = wh.GetDouble();
            if (Current.OldProperties.TryGetValue("window_left", out var wl)) Current.Window.Left = wl.GetDouble();
            if (Current.OldProperties.TryGetValue("window_top", out var wt)) Current.Window.Top = wt.GetDouble();
            if (Current.OldProperties.TryGetValue("is_window_maximized", out var iwm)) Current.Window.IsMaximized = iwm.GetBoolean();
        }

        Current.ConfigVersion = 2;
    }
}