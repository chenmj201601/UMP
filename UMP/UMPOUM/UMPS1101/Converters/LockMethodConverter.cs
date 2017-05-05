//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    946f76f6-9f2c-4178-95f6-e1ccab0f17aa
//        CLR Version:              4.0.30319.18444
//        Name:                     LockMethodConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                LockMethodConverter
//
//        created by Charley at 2014/9/3 16:58:19
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1101.Converters
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
                    image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/userlocked.png", UriKind.Relative);
                    image.EndInit();
                    return image;
                case "L":
                    image.BeginInit();
                    image.UriSource = new Uri("/UMPS1101;component/Themes/Default/UMPS1101/Images/locked.png", UriKind.Relative);
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
