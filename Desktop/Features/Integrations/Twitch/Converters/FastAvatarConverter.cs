using StreamTabula.Features.Integrations.Twitch.Exceptions;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StreamTabula.Features.Integrations.Twitch.Converters;

public class FastAvatarConverter : IValueConverter
{
    private const int DefaultDecodeWidth = 48;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrWhiteSpace(url))
            return null;

        try
        {
            return CreateFrozenBitmap(url, GetDecodeWidth(parameter));
        }
        catch (Exception ex)
        {
            throw new AvatarLoadException($"Failed to load avatar from '{url}'.", ex);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static BitmapImage CreateFrozenBitmap(string url, int decodeWidth)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();

        bitmap.UriSource = new Uri(url);
        bitmap.DecodePixelWidth = decodeWidth;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;

        bitmap.EndInit();

        if (bitmap.CanFreeze)
        {
            bitmap.Freeze();
        }

        return bitmap;
    }

    private static int GetDecodeWidth(object parameter)
    {
        if (parameter != null && int.TryParse(parameter.ToString(), out int width))
        {
            return width;
        }
        return DefaultDecodeWidth;
    }
}