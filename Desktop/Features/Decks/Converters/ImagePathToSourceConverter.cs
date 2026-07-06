using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace StreamTabula.Features.Decks.Converters;

public class ImagePathToSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path))
            return null;

        if (Path.IsPathRooted(path))
        {
            return path;
        }

        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDir, "Assets", "Images", "Buttons", path);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}