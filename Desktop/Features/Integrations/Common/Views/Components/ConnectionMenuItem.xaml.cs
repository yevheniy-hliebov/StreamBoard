using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StreamBoard.Components.Navigation;
using StreamBoard.Features.Integrations.Common.Models;

namespace StreamBoard.Features.Integrations.Common.Views.Components
{
    public partial class ConnectionMenuItem : MenuItem
    {
        public ConnectionMenuItem()
        {
            InitializeComponent();
            UpdateState();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ConnectionMenuItem));

        public ConnectionState State
        {
            get => (ConnectionState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ConnectionState), typeof(ConnectionMenuItem),
                new PropertyMetadata(ConnectionState.NotConnected, OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ConnectionMenuItem control) control.UpdateState();
        }
        public Brush StateColor
        {
            get => (Brush)GetValue(StateColorProperty);
            set => SetValue(StateColorProperty, value);
        }
        public static readonly DependencyProperty StateColorProperty =
            DependencyProperty.Register(nameof(StateColor), typeof(Brush), typeof(ConnectionMenuItem));

        public string StateText
        {
            get => (string)GetValue(StateTextProperty);
            set => SetValue(StateTextProperty, value);
        }
        public static readonly DependencyProperty StateTextProperty =
            DependencyProperty.Register(nameof(StateText), typeof(string), typeof(ConnectionMenuItem), new PropertyMetadata("Unknown"));

        public Type TargetPageType
        {
            get => (Type)GetValue(TargetPageTypeProperty);
            set => SetValue(TargetPageTypeProperty, value);
        }
        public static readonly DependencyProperty TargetPageTypeProperty =
            DependencyProperty.Register(nameof(TargetPageType), typeof(Type), typeof(ConnectionMenuItem));

        protected override void OnClick()
        {
            base.OnClick();

            if (TargetPageType != null)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow == null) return;

                var navComponent = mainWindow.FindName("RootNavView") as MainNavView;
                var navigationControl = navComponent?.GetNavigation();

                if (navigationControl == null) return;

                navigationControl.Navigate(TargetPageType);
            }
        }

        private void UpdateState()
        {
            (string hex, string text) = State switch
            {
                ConnectionState.Connected => ("#5BFE76", "Connected"),
                ConnectionState.Connecting => ("#FED85B", "Connecting..."),
                ConnectionState.Failed => ("#FE5B5B", "Connection Failed"),
                ConnectionState.Disconnecting => ("#DADADA", "Disconnecting..."),
                _ => ("#FE5B5B", "Not Connected")
            };

            var color = (Color)ColorConverter.ConvertFromString(hex);
            var brush = new SolidColorBrush(color);
            brush.Freeze();

            SetValue(StateColorProperty, brush);
            SetValue(StateTextProperty, text);
        }
    }
}