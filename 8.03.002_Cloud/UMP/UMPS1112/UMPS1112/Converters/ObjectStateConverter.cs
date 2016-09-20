using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1112.Converters
{
    public class ObjectStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage image = new BitmapImage();
            if (value == null)
            {
                return null;
            }
            int orgType;
            bool? isActive = value as bool?;
            if (isActive==true)
            {
                orgType = 1;
            }
            else if (isActive == false)
            {
                orgType = 0;
            }
            else
            {
                orgType = 2;
            }
            
            //if (int.TryParse(value.ToString(), out orgType))
            {
                switch (orgType)
                {
                    case 1:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1112/Images/avaliable.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 2:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1112/Images/warning.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 0:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1112/Images/disabled.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
