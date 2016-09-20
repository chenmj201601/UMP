using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1103.Converters
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
            if (int.TryParse(value.ToString(), out orgType))
            {
                switch (orgType)
                {
                    case 1:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1103;component/Themes/Default/UMPS1103/Images/avaliable.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 2:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1103;component/Themes/Default/UMPS1103/Images/warning.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 0:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1103;component/Themes/Default/UMPS1103/Images/disabled.ico", UriKind.Relative);
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
