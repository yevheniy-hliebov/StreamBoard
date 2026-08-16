using System.Windows;
using StreamTabula.Features.Settings.Services;

namespace StreamTabula.Shell.Behaviors;

public class WindowBoundsManager
{
    private readonly Window _window;
    private readonly SettingsStorage _settings;

    public WindowBoundsManager(Window window, SettingsStorage settings)
    {
        _window = window;
        _settings = settings;

        _window.Loaded += OnLoaded;
        _window.Closing += OnClosing;
        _window.StateChanged += OnStateChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var config = _settings.Current;

        if (config.Startup.StartMinimized)
            _window.WindowState = WindowState.Minimized;

        if (config.Window.Width <= 0 || config.Window.Height <= 0)
        {
            _window.Width = SystemParameters.WorkArea.Width * 0.8;
            _window.Height = SystemParameters.WorkArea.Height * 0.8;
            _window.Left = (SystemParameters.WorkArea.Width - _window.Width) / 2 + SystemParameters.WorkArea.Left;
            _window.Top = (SystemParameters.WorkArea.Height - _window.Height) / 2 + SystemParameters.WorkArea.Top;
        }
        else
        {
            _window.Width = config.Window.Width;
            _window.Height = config.Window.Height;
            _window.Left = config.Window.Left;
            _window.Top = config.Window.Top;

            if (_window.Left < SystemParameters.VirtualScreenLeft || _window.Left >= SystemParameters.VirtualScreenWidth ||
                _window.Top < SystemParameters.VirtualScreenTop || _window.Top >= SystemParameters.VirtualScreenHeight)
            {
                _window.Left = (SystemParameters.WorkArea.Width - _window.Width) / 2 + SystemParameters.WorkArea.Left;
                _window.Top = (SystemParameters.WorkArea.Height - _window.Height) / 2 + SystemParameters.WorkArea.Top;
            }

            if (config.Window.IsMaximized)
                _window.WindowState = WindowState.Maximized;
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (_settings.Current.Startup.MinimizeToTray && _window.WindowState == WindowState.Minimized)
        {
            _window.Hide();
        }
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var config = _settings.Current;

        if (_window.WindowState == WindowState.Maximized)
        {
            config.Window.IsMaximized = true;
            config.Window.Width = _window.RestoreBounds.Width;
            config.Window.Height = _window.RestoreBounds.Height;
            config.Window.Left = _window.RestoreBounds.Left;
            config.Window.Top = _window.RestoreBounds.Top;
        }
        else if (_window.WindowState == WindowState.Normal)
        {
            config.Window.IsMaximized = false;
            config.Window.Width = _window.Width;
            config.Window.Height = _window.Height;
            config.Window.Left = _window.Left;
            config.Window.Top = _window.Top;
        }
        else if (_window.WindowState == WindowState.Minimized)
        {
            config.Window.Width = _window.RestoreBounds.Width;
            config.Window.Height = _window.RestoreBounds.Height;
            config.Window.Left = _window.RestoreBounds.Left;
            config.Window.Top = _window.RestoreBounds.Top;
        }

        _settings.Save();
    }
}