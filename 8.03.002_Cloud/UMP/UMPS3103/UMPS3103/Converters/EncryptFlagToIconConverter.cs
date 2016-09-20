using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UMPS3103.Converters
{
    public class EncryptFlagToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            string strFlag = value.ToString();
            if (strFlag == "2")
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
