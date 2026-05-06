using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StreamTabula.Features.Decks.Views.Components.Layout
{
    public partial class DeckEditorLayout : UserControl
    {
        // Зберігаємо останні розміри
        private double _lastLibraryWidth = 200;
        private double _lastPropertiesHeight = 237;

        public DeckEditorLayout() => InitializeComponent();

        // --- ВЛАСТИВОСТІ КОНТЕНТУ ---
        public object LeftBarContent
        {
            get => GetValue(LeftBarContentProperty);
            set => SetValue(LeftBarContentProperty, value);
        }
        public static readonly DependencyProperty LeftBarContentProperty =
            DependencyProperty.Register(nameof(LeftBarContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));

        public object CanvasContent
        {
            get => GetValue(CanvasContentProperty);
            set => SetValue(CanvasContentProperty, value);
        }
        public static readonly DependencyProperty CanvasContentProperty =
            DependencyProperty.Register(nameof(CanvasContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));

        public object PropertiesContent
        {
            get => GetValue(PropertiesContentProperty);
            set => SetValue(PropertiesContentProperty, value);
        }
        public static readonly DependencyProperty PropertiesContentProperty =
            DependencyProperty.Register(nameof(PropertiesContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));

        public object LibraryContent
        {
            get => GetValue(LibraryContentProperty);
            set => SetValue(LibraryContentProperty, value);
        }
        public static readonly DependencyProperty LibraryContentProperty =
            DependencyProperty.Register(nameof(LibraryContent), typeof(object), typeof(DeckEditorLayout), new PropertyMetadata(null));


        // --- ВЛАСТИВІСТЬ АНІМАЦІЇ РЕЖИМУ ---
        public bool IsClickMode
        {
            get => (bool)GetValue(IsClickModeProperty);
            set => SetValue(IsClickModeProperty, value);
        }

        public static readonly DependencyProperty IsClickModeProperty =
            DependencyProperty.Register(nameof(IsClickMode), typeof(bool), typeof(DeckEditorLayout),
                new PropertyMetadata(false, OnIsClickModeChanged));

        private static void OnIsClickModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeckEditorLayout layout)
            {
                layout.AnimateMode((bool)e.NewValue);
            }
        }

        private void AnimateMode(bool isClickMode)
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };
            var duration = TimeSpan.FromSeconds(0.35);

            if (isClickMode)
            {
                // Зберігаємо розміри
                if (ColLibrary.ActualWidth > 10) _lastLibraryWidth = ColLibrary.ActualWidth;
                if (RowProperties.ActualHeight > 10) _lastPropertiesHeight = RowProperties.ActualHeight;

                ColLibrary.MinWidth = 0;
                RowProperties.MinHeight = 0;

                // Ховаємо
                ColLibrary.BeginAnimation(ColumnDefinition.MaxWidthProperty, new DoubleAnimation(_lastLibraryWidth, 0, duration) { EasingFunction = ease });
                RowProperties.BeginAnimation(RowDefinition.MaxHeightProperty, new DoubleAnimation(_lastPropertiesHeight, 0, duration) { EasingFunction = ease });

                ColLibrarySplitter.MaxWidth = 0;
                RowPropertiesSplitter.MaxHeight = 0;

                LibraryBorder.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2)));
                PropertiesBorder.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2)));
            }
            else
            {
                // Показуємо
                var animCol = new DoubleAnimation(0, _lastLibraryWidth, duration) { EasingFunction = ease };
                animCol.Completed += (s, ev) =>
                {
                    ColLibrary.MinWidth = 200;
                    ColLibrary.BeginAnimation(ColumnDefinition.MaxWidthProperty, null);
                };
                ColLibrary.BeginAnimation(ColumnDefinition.MaxWidthProperty, animCol);

                var animRow = new DoubleAnimation(0, _lastPropertiesHeight, duration) { EasingFunction = ease };
                animRow.Completed += (s, ev) =>
                {
                    RowProperties.MinHeight = 237;
                    RowProperties.BeginAnimation(RowDefinition.MaxHeightProperty, null);
                };
                RowProperties.BeginAnimation(RowDefinition.MaxHeightProperty, animRow);

                ColLibrarySplitter.MaxWidth = 5;
                RowPropertiesSplitter.MaxHeight = 5;

                LibraryBorder.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3)) { BeginTime = TimeSpan.FromSeconds(0.1) });
                PropertiesBorder.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3)) { BeginTime = TimeSpan.FromSeconds(0.1) });
            }
        }
    }
}