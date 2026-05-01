using System.Windows;
using Wpf.Ui.Controls;

namespace StreamBoard.Components.Dialogs
{
    public partial class ConfirmationDialogWindow : FluentWindow
    {
        public string Message { get; }

        public ConfirmationDialogWindow(string title, string message)
        {
            InitializeComponent();

            Title = title;
            Message = message;

            DataContext = this;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
