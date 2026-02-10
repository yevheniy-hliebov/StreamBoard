using StreamBoard.Features.Home.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StreamBoard.Components.Navigation
{
    /// <summary>
    /// Interaction logic for MainNavView.xaml
    /// </summary>
    public partial class MainNavView : UserControl
    {
        public Wpf.Ui.Controls.NavigationView GetNavigation() => RootNavigation;

        public MainNavView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            
            RootNavigation.Navigate(typeof(HomePage));
        }
    }
}
