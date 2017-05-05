using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UMPS3106.LinkToLaosURL
{
    public class LaosLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int flag;
            if (int.TryParse(value.ToString(), out flag))
            {
                var param = parameter.ToString();
                if (string.IsNullOrEmpty(param))
                {
                    return Visibility.Collapsed;
                }
                if (param == "Link" || param == "Browse")
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
