using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace StreamBoard.Helpers
{
    public class StatusCodeToAppearanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int code)
            {
                if (code >= 200 && code < 300)
                    return ControlAppearance.Success;

                if (code >= 300 && code < 400)
                    return ControlAppearance.Info;   

                if (code >= 400 && code < 500)
                    return ControlAppearance.Caution;

                if (code >= 500)
                    return ControlAppearance.Danger;
            }

            return ControlAppearance.Secondary;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
