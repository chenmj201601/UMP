//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d5e3f3ef-5069-4b7b-875f-b5cb65d43510
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectStateConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                ObjectStateConverter
//
//        created by Charley at 2014/8/27 10:27:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1101.Converters
{
    public class ObjectStateConverter:IValueConverter
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
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/avaliable.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 2:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/warning.ico", UriKind.Relative);
                        image.EndInit();
                        return image;
                    case 0:
                        image.BeginInit();
                        image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/disabled.ico", UriKind.Relative);
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
