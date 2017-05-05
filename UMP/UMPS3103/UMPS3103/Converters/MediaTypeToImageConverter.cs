using System;
using System.Windows;
using System.Windows.Data;

namespace UMPS3103.Converters
{
    public class MediaTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }
            int mediaType;
            string strValue = value.ToString();
            string strParam = parameter.ToString();
            if (!int.TryParse(strValue, out mediaType))
            {
                return Visibility.Collapsed;
            }
            if (mediaType == 0
                && strParam == "A")
            {
                return Visibility.Visible;
            }
            if (mediaType == 1
                && strParam == "V")
            {
                return Visibility.Visible;
            }
            if (mediaType == 2
                && strParam == "S")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
