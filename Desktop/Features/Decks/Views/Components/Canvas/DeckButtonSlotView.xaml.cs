using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StreamTabula.Features.Decks.Models;

namespace StreamTabula.Features.Decks.Views.Components.Canvas
{
    public partial class DeckButtonSlotView : UserControl
    {
        public DeckButtonSlotView() => InitializeComponent();

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DeckButtonSlotView), new PropertyMetadata(false));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DeckButtonSlotView), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(DeckButtonSlotView), new PropertyMetadata(null));

        public int ButtonIndex
        {
            get => (int)GetValue(ButtonIndexProperty);
            set => SetValue(ButtonIndexProperty, value);
        }
        public static readonly DependencyProperty ButtonIndexProperty =
            DependencyProperty.Register(nameof(ButtonIndex), typeof(int), typeof(DeckButtonSlotView), new PropertyMetadata(0));

        public DeckButtonConfig? Config
        {
            get => (DeckButtonConfig?)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }

        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register(
                nameof(Config),
                typeof(DeckButtonConfig),
                typeof(DeckButtonSlotView),
                new PropertyMetadata(null, OnConfigChanged));

        public bool IsImageMissing
        {
            get => (bool)GetValue(IsImageMissingProperty);
            private set => SetValue(IsImageMissingPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsImageMissingPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsImageMissing),
                typeof(bool),
                typeof(DeckButtonSlotView),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsImageMissingProperty = IsImageMissingPropertyKey.DependencyProperty;

        private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeckButtonSlotView slotView)
            {
                if (e.OldValue is DeckButtonConfig oldConfig)
                {
                    oldConfig.PropertyChanged -= slotView.Config_PropertyChanged;
                }

                if (e.NewValue is DeckButtonConfig newConfig)
                {
                    newConfig.PropertyChanged += slotView.Config_PropertyChanged;
                    slotView.UpdateImageStatus(newConfig.ImagePath);
                }
                else
                {
                    slotView.UpdateImageStatus(null);
                }
            }
        }

        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeckButtonConfig.ImagePath) && sender is DeckButtonConfig config)
            {
                UpdateImageStatus(config.ImagePath);
            }
        }

        private void UpdateImageStatus(string? path)
        {
            IsImageMissing = !string.IsNullOrWhiteSpace(path) && !File.Exists(path);
        }
    }
}