//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fd943b8b-1356-469d-9866-24902e4efd38
//        CLR Version:              4.0.30319.18444
//        Name:                     OrgTypeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                OrgTypeConverter
//
//        created by Charley at 2014/8/27 10:03:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1101.Converters
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
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/root.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 902:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/company.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 903:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/group.ico", UriKind.Relative);
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
