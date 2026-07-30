using System.Globalization;
using System.Windows.Data;

namespace StreamTabula.Helpers
{
    public class ToolTipFallbackConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tooltip = values[0] as string;
            var fallback = values[1]?.ToString();

            return string.IsNullOrWhiteSpace(tooltip) ? fallback : tooltip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
