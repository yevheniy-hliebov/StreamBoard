using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace StreamTabula.Features.Servers.Services
{
    public static class QrHelper
    {
        public static BitmapImage GenerateUrlQrCode(string ipAddress, int port)
        {
            string url = $"http://{ipAddress}:{port}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeBytes = qrCode.GetGraphic(20);

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
}
