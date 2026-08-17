using Microsoft.Extensions.DependencyInjection;
using StreamTabula.Features.Integrations.Common.Models;
using StreamTabula.Features.Navigation.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StreamTabula.Features.Integrations.Common.Views.Controls;

public partial class IntegrationMenuItem : UserControl
{
    private readonly NavigationService _navService;

    public IntegrationMenuItem()
    {
        InitializeComponent();
        _navService = App.ServiceProvider.GetRequiredService<NavigationService>();
    }

    public IntegrationStatusModel Integration
    {
        get => (IntegrationStatusModel)GetValue(IntegrationProperty);
        set => SetValue(IntegrationProperty, value);
    }
    public static readonly DependencyProperty IntegrationProperty =
        DependencyProperty.Register(nameof(Integration), typeof(IntegrationStatusModel), typeof(IntegrationMenuItem),
            new PropertyMetadata(null, OnIntegrationChanged));

    private static void OnIntegrationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IntegrationMenuItem control)
        {
            if (e.OldValue is IntegrationStatusModel oldModel)
                oldModel.PropertyChanged -= control.OnModelPropertyChanged;

            if (e.NewValue is IntegrationStatusModel newModel)
                newModel.PropertyChanged += control.OnModelPropertyChanged;

            control.UpdateStatus();
        }
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IntegrationStatusModel.Status))
        {
            Dispatcher.BeginInvoke(UpdateStatus);
        }
    }

    public Brush StatusColor
    {
        get => (Brush)GetValue(StatusColorProperty);
        set => SetValue(StatusColorProperty, value);
    }
    public static readonly DependencyProperty StatusColorProperty =
        DependencyProperty.Register(nameof(StatusColor), typeof(Brush), typeof(IntegrationMenuItem));

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }
    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(IntegrationMenuItem), new PropertyMetadata("Unknown"));

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (Integration?.TargetPageType != null)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                _navService.NavigateTo(Integration.TargetPageType);
            }
        }

        CloseParentContextMenu(this);
        e.Handled = true;
    }

    private void UpdateStatus()
    {
        var currentStatus = Integration?.Status ?? ConnectionStatus.NotConnected;

        (string hex, string text) = currentStatus switch
        {
            ConnectionStatus.Connected => ("#5BFE76", "Connected"),
            ConnectionStatus.Connecting => ("#FED85B", "Connecting..."),
            ConnectionStatus.Failed => ("#FE5B5B", "Connection Failed"),
            ConnectionStatus.Disconnecting => ("#DADADA", "Disconnecting..."),
            _ => ("#FE5B5B", "Not Connected")
        };

        var color = (Color)ColorConverter.ConvertFromString(hex);
        var brush = new SolidColorBrush(color);
        brush.Freeze();

        SetValue(StatusColorProperty, brush);
        SetValue(StatusTextProperty, text);
    }

    private void CloseParentContextMenu(DependencyObject current)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(current);
        while (parent != null)
        {
            if (parent is ContextMenu menu)
            {
                menu.IsOpen = false;
                return;
            }
            parent = VisualTreeHelper.GetParent(parent);
        }
    }
}