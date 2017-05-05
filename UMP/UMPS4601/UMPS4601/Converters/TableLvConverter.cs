using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using VoiceCyber.UMP.Common;

namespace UMPS4601.Converters
{
    public class TableLvConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }
            //double trendType;
            double doubleValue = double.Parse(value.ToString());
            if (value.Equals(string.Empty))
            {
                return Visibility.Collapsed;
            }
            string strParam = parameter.ToString();
            //if (!double.TryParse(strValue, out trendType))
            //{
            //    return Visibility.Collapsed;
            //}
            if (doubleValue==1&& strParam == "U")//1 上升
            {
                return Visibility.Visible;
            }
            if (doubleValue==-1 && strParam == "D")//-1下降
            {
                return Visibility.Visible;
            }
            if (doubleValue==0 && strParam == "F")//0持平
            {
                return Visibility.Visible;
            }
            if (doubleValue >= 1 && strParam == "G")//达标
            {
                return Visibility.Visible;
            }
            if (doubleValue < 1 && strParam == "B")//不达标
            {
                return Visibility.Visible;
            }
            if (strParam == "Go")
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
