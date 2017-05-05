using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UMPS3104.Converters
{
    public class BookTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                && strParam == "D")
            {
                return Visibility.Visible;
            }
            if (mediaType == 1
                && strParam == "W")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
