using System;
using System.Text;
using System.Windows.Data;
using System.Windows;


namespace UMPS1102.Converters
{
    public class PermissionIsCheckConvert: IValueConverter
    {
        public object Convert(object o, Type type, object parameter,
                            System.Globalization.CultureInfo culture)
        {
            
                if (o.ToString().Equals("1"))
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            
        }
        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="o"></param>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public object ConvertBack(object o, Type type, object parameter, System.Globalization.CultureInfo culture)
        {

            if((bool)o)
            {
                return "1";
            }
            else
            {
                return "0";
            }
            //throw new NotSupportedException();
        }
    }
}
