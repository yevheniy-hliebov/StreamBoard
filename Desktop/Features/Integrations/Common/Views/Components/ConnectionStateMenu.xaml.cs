using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Integrations.Common.Views.Components
{
    public partial class ConnectionStateMenu : UserControl
    {
        public ConnectionStateMenu() => InitializeComponent();

        public IEnumerable<IntegrationStateModel> Items
        {
            get => (IEnumerable<IntegrationStateModel>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable<IntegrationStateModel>), typeof(ConnectionStateMenu),
                new PropertyMetadata(null, OnItemsChanged));

        public string SummaryText
        {
            get => (string)GetValue(SummaryTextProperty);
            set => SetValue(SummaryTextProperty, value);
        }
        public static readonly DependencyProperty SummaryTextProperty =
            DependencyProperty.Register(nameof(SummaryText), typeof(string), typeof(ConnectionStateMenu));

        public Brush SummaryColor
        {
            get => (Brush)GetValue(SummaryColorProperty);
            set => SetValue(SummaryColorProperty, value);
        }
        public static readonly DependencyProperty SummaryColorProperty =
            DependencyProperty.Register(nameof(SummaryColor), typeof(Brush), typeof(ConnectionStateMenu));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionStateMenu control)
            {
                if (e.OldValue is IEnumerable<IntegrationStateModel> oldList)
                {
                    if (oldList is INotifyCollectionChanged notify)
                        notify.CollectionChanged -= control.Items_CollectionChanged;

                    foreach (var item in oldList)
                        item.PropertyChanged -= control.OnItemPropertyChanged;
                }

                if (e.NewValue is IEnumerable<IntegrationStateModel> newList)
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
            if (e.PropertyName == nameof(IntegrationStateModel.State))
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
                foreach (IntegrationStateModel item in e.NewItems)
                    item.PropertyChanged += OnItemPropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (IntegrationStateModel item in e.OldItems)
                    item.PropertyChanged -= OnItemPropertyChanged;
            }

            UpdateSummary();
        }

        public void UpdateSummary()
        {
            if (Items == null || !Items.Any())
            {
                SummaryText = "0/0";
                SummaryColor = Brushes.Gray;
                return;
            }

            int total = Items.Count();
            int connected = Items.Count(i => i.State == ConnectionState.Connected);

            SummaryText = $"{connected}/{total}";

            if (connected == total)
                SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#5BFE76")!;
            else if (connected > 0)
                SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FED85B")!;
            else
                SummaryColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FE5B5B")!;
        }
    }
}