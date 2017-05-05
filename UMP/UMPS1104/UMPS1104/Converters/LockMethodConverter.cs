using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1104.Converters
{
    public class LockMethodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            BitmapImage image = new BitmapImage();
            string lockMethod = value.ToString();
            switch (lockMethod)
            {
                case "U":
                    image.BeginInit();
                    image.UriSource = new Uri("/UMPS1104;component/Themes/Default/UMPS1101/Images/userlocked.png", UriKind.Relative);
                    image.EndInit();
                    return image;
                case "L":
                    image.BeginInit();
                    image.UriSource = new Uri("/UMPS1104;component/Themes/Default/UMPS1101/Images/locked.png", UriKind.Relative);
                    image.EndInit();
                    return image;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
