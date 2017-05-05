using System;
using UMPS1102.Models;
using System.Windows.Data;
using System.Windows;

namespace UMPS1102.Converters
{
    public  class PermissionIsEnable:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObjectItem obj = value as ObjectItem;
            if (obj == null)
            {
                return false;

            }
            if ((bool)obj.IsChecked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
