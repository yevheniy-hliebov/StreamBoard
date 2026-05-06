using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StreamTabula.Helpers
{
    public class FastAvatarConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string url || string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.UriSource = new Uri(url);

                bitmap.DecodePixelWidth = 48;

                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();

                if (bitmap.CanFreeze)
                {
                    bitmap.Freeze();
                }

                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
