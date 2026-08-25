using QRCoder;
using System.IO;
using System.Windows.Media.Imaging;

namespace StreamTabula.Features.Servers.Services;

public interface IQrGenerator
{
    BitmapImage Generate(string content);
}

public class QrGenerator : IQrGenerator
{
    private const int PixelsPerModule = 20;

    public BitmapImage Generate(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);

        byte[] qrCodeBytes = qrCode.GetGraphic(PixelsPerModule);

        var bitmapImage = new BitmapImage();
        using (var stream = new MemoryStream(qrCodeBytes))
        {
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
        }

        return bitmapImage;
    }
}
