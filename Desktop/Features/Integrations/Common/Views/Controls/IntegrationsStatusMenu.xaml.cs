using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StreamTabula.Features.Integrations.Common.Models;

namespace StreamTabula.Features.Integrations.Common.Views.Controls;

public partial class IntegrationsStatusMenu : UserControl
{
    public IntegrationsStatusMenu() => InitializeComponent();

    public IEnumerable<IntegrationStatusModel> Integrations
    {
        get => (IEnumerable<IntegrationStatusModel>)GetValue(IntegrationsProperty);
        set => SetValue(IntegrationsProperty, value);
    }
    public static readonly DependencyProperty IntegrationsProperty =
        DependencyProperty.Register(nameof(Integrations), typeof(IEnumerable<IntegrationStatusModel>), typeof(IntegrationsStatusMenu),
            new PropertyMetadata(null, OnItemsChanged));

    public string SummaryText
    {
        get => (string)GetValue(SummaryTextProperty);
        set => SetValue(SummaryTextProperty, value);
    }
    public static readonly DependencyProperty SummaryTextProperty =
        DependencyProperty.Register(nameof(SummaryText), typeof(string), typeof(IntegrationsStatusMenu));

    public Brush SummaryColor
    {
        get => (Brush)GetValue(SummaryColorProperty);
        set => SetValue(SummaryColorProperty, value);
    }
    public static readonly DependencyProperty SummaryColorProperty =
        DependencyProperty.Register(nameof(SummaryColor), typeof(Brush), typeof(IntegrationsStatusMenu));

    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IntegrationsStatusMenu control)
        {
            if (e.OldValue is IEnumerable<IntegrationStatusModel> oldList)
            {
                if (oldList is INotifyCollectionChanged notify)
                    notify.CollectionChanged -= control.Items_CollectionChanged;

                foreach (var item in oldList)
                    item.PropertyChanged -= control.OnItemPropertyChanged;
            }

            if (e.NewValue is IEnumerable<IntegrationStatusModel> newList)
            {
                if (newList is INotifyCollectionChanged notify)
                    notify.CollectionChanged += control.Items_CollectionChanged;

                foreach (var item in newList)
                    item.PropertyChanged += control.OnItemPropertyChanged;
            }

            control.UpdateSummary();
        }
    }

    private void OnItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IntegrationStatusModel.Status))
        {
            Dispatcher.BeginInvoke(() =>
            {
                UpdateSummary();
            });
        }
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (IntegrationStatusModel item in e.NewItems)
                item.PropertyChanged += OnItemPropertyChanged;
        }

        if (e.OldItems != null)
        {
            foreach (IntegrationStatusModel item in e.OldItems)
                item.PropertyChanged -= OnItemPropertyChanged;
        }

        UpdateSummary();
    }

    public void UpdateSummary()
    {
        if (Integrations == null || !Integrations.Any())
        {
            SummaryText = "0/0";
            SummaryColor = Brushes.Gray;
            return;
        }

        int total = Integrations.Count();
        int connected = Integrations.Count(i => i.Status == ConnectionStatus.Connected);

        SummaryText = $"{connected}/{total}";

        if (connected == total)
            SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#5BFE76")!;
        else if (connected > 0)
            SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FED85B")!;
        else
            SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FE5B5B")!;
    }
}