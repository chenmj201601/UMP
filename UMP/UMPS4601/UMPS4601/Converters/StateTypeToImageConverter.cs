using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UMPS4601.Converters
{
    public class StateTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }
            int stateType;
            string strValue = value.ToString();
            string strParam = parameter.ToString();
            if (!int.TryParse(strValue, out stateType))
            {
                return Visibility.Collapsed;
            }
            if (strValue == "0" && strParam == "F")
            {
                return Visibility.Visible;
            }
            if (strValue == "1" && strParam == "S")
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
