using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS5101.Converters
{
    public class KeywordStateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            BitmapImage image = new BitmapImage();
            string state = value.ToString();
            switch (state)
            {
                case "0":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00002.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "1":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00001.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "3":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00003.png", UriKind.RelativeOrAbsolute);
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
