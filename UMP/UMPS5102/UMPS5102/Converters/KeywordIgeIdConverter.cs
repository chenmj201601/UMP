using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS5102.Converters
{
    public class KeywordStrImageConverter : IValueConverter
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
                case "00011.png":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00011.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "00012.png":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00012.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "00013.png":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00013.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "00014.png":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00014.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "00015.png":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/00015.png", UriKind.RelativeOrAbsolute);
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
