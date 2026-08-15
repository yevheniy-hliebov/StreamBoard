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

        if (config.StartMinimized)
            _window.WindowState = WindowState.Minimized;

        if (config.WindowWidth <= 0 || config.WindowHeight <= 0)
        {
            _window.Width = SystemParameters.WorkArea.Width * 0.8;
            _window.Height = SystemParameters.WorkArea.Height * 0.8;
            _window.Left = (SystemParameters.WorkArea.Width - _window.Width) / 2 + SystemParameters.WorkArea.Left;
            _window.Top = (SystemParameters.WorkArea.Height - _window.Height) / 2 + SystemParameters.WorkArea.Top;
        }
        else
        {
            _window.Width = config.WindowWidth;
            _window.Height = config.WindowHeight;
            _window.Left = config.WindowLeft;
            _window.Top = config.WindowTop;

            if (_window.Left < SystemParameters.VirtualScreenLeft || _window.Left >= SystemParameters.VirtualScreenWidth ||
                _window.Top < SystemParameters.VirtualScreenTop || _window.Top >= SystemParameters.VirtualScreenHeight)
            {
                _window.Left = (SystemParameters.WorkArea.Width - _window.Width) / 2 + SystemParameters.WorkArea.Left;
                _window.Top = (SystemParameters.WorkArea.Height - _window.Height) / 2 + SystemParameters.WorkArea.Top;
            }

            if (config.IsWindowMaximized)
                _window.WindowState = WindowState.Maximized;
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (_settings.Current.MinimizeToTray && _window.WindowState == WindowState.Minimized)
        {
            _window.Hide();
        }
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var config = _settings.Current;

        if (_window.WindowState == WindowState.Maximized)
        {
            config.IsWindowMaximized = true;
            config.WindowWidth = _window.RestoreBounds.Width;
            config.WindowHeight = _window.RestoreBounds.Height;
            config.WindowLeft = _window.RestoreBounds.Left;
            config.WindowTop = _window.RestoreBounds.Top;
        }
        else if (_window.WindowState == WindowState.Normal)
        {
            config.IsWindowMaximized = false;
            config.WindowWidth = _window.Width;
            config.WindowHeight = _window.Height;
            config.WindowLeft = _window.Left;
            config.WindowTop = _window.Top;
        }
        else if (_window.WindowState == WindowState.Minimized)
        {
            config.WindowWidth = _window.RestoreBounds.Width;
            config.WindowHeight = _window.RestoreBounds.Height;
            config.WindowLeft = _window.RestoreBounds.Left;
            config.WindowTop = _window.RestoreBounds.Top;
        }

        _settings.Save();
    }
}