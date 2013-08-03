using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using System.Globalization;

namespace ARDrone2Client.Windows.ViewModel
{
    public class StringToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            float parsedValue = 0.0f;
            if (value is string)
                float.TryParse((string)value, out parsedValue);

            return parsedValue;
        }

        public object Convert(object value, Type targetType,
                      object parameter, string culture)
        {
            float parsedValue = 0.0f;
            if (value is string)
                float.TryParse((string)value, out parsedValue);

            return parsedValue;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType,
                          object parameter, string culture)
        {
            return value.ToString();
        }
    }
}
