using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPS3105.Models;

namespace UMPS3105.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var list = parameter as List<EnumItem>;
            if (list != null)
            {
                EnumItem temp = list.FirstOrDefault(e => e.Name == value.ToString());
                if (temp != null)
                {
                    temp.IsSelected = true;
                    return temp;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EnumItem enumItem = value as EnumItem;
            if (enumItem == null)
            {
                return 0;
            }
            var enumValue = Enum.Parse(enumItem.Type, enumItem.Name);
            return enumValue;
        }
    }
}
