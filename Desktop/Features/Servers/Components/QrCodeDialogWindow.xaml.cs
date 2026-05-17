using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace StreamTabula.Features.Servers.Components
{
    public partial class QrCodeDialogWindow : FluentWindow
    {
        public string Message { get; }
        public BitmapImage QrImage { get; }

        public QrCodeDialogWindow(string title, string message, BitmapImage qrImage)
        {
            InitializeComponent();

            Title = title;
            Message = message;
            QrImage = qrImage;

            DataContext = this;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}