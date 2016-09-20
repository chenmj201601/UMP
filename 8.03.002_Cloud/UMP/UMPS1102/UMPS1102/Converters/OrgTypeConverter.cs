using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1102.Converters
{
    public class OrgTypeConverter : IValueConverter
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
                    case 901:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1102/Images/root.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 902:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1102/Images/company.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 903:
                        image.BeginInit();
                        image.UriSource = new Uri("/Themes/Default/UMPS1102/Images/group.ico", UriKind.Relative);
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
