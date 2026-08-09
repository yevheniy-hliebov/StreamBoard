using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace StreamTabula.Features.Servers.Converters;

public class StatusCodeToAppearanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int code)
            return ControlAppearance.Secondary;

        return code switch
        {
            >= 200 and < 300 => ControlAppearance.Success,
            >= 300 and < 400 => ControlAppearance.Info,
            >= 400 and < 500 => ControlAppearance.Caution,
            >= 500 => ControlAppearance.Danger,
            _ => ControlAppearance.Secondary
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}