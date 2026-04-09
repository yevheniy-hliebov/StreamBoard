using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Integrations.Common.Views.Components
{
    public partial class IntegrationIcon : ImageIcon
    {
        public IntegrationIcon()
        {
            InitializeComponent();
            UpdateIconSource(IconType);

            Width = Size;
            Height = Size;
        }

        public IntegrationIconType? IconType
        {
            get => (IntegrationIconType?)GetValue(IconTypeProperty);
            set => SetValue(IconTypeProperty, value);
        }

        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register(
                nameof(IconType),
                typeof(IntegrationIconType?),
                typeof(IntegrationIcon),
                new PropertyMetadata(null, OnIconTypeChanged));

        private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegrationIcon control)
            {
                control.UpdateIconSource(e.NewValue as IntegrationIconType?);
            }
        }

        private void UpdateIconSource(IntegrationIconType? type)
        {
            if (type == null)
            {
                Source = null;
                Visibility = Visibility.Collapsed;
                return;
            }

            string? fileName = type?.ToString().ToLower();
            if (string.IsNullOrEmpty(fileName))
            {
                Visibility = Visibility.Collapsed;
                return;
            }

            string uriString = $"pack://application:,,,/Assets/Images/Integrations/{fileName}.png";

            try
            {
                Source = new BitmapImage(new Uri(uriString));
                Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Source = null;
                Visibility = Visibility.Collapsed;
            }
        }

        public int Size
        {
            get => (int)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(
                nameof(Size),
                typeof(int),
                typeof(IntegrationIcon),
                new PropertyMetadata(16, OnSizeChanged));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IntegrationIcon control && e.NewValue is int newSize)
            {
                control.Width = newSize;
                control.Height = newSize;
            }
        }
    }
}