using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1102.Converters
{
    public class ImageSourceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(value.ToString(), UriKind.Relative);
                image.EndInit();
                return image;
            }
            catch { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
