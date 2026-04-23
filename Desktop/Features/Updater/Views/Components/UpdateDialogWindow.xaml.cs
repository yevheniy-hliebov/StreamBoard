using System.Windows;
using Wpf.Ui.Controls;

namespace StreamBoard.Features.Updater.Views.Components
{
    public partial class UpdateDialogWindow : FluentWindow
    {
        public UpdateDialogWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}