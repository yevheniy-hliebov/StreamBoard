using StreamTabula.Controls.Dialogs;
using System.Windows.Media.Imaging;

namespace StreamTabula.Features.Servers.Components;

public partial class QrCodeDialogWindow : BaseDialog
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
}