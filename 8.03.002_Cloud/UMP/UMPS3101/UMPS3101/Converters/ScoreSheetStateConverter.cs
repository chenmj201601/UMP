//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5dfc933a-4a85-4fb0-b451-a3fdca1c3240
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetStateConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                ScoreSheetStateConverter
//
//        created by Charley at 2014/10/9 16:09:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS3101.Converters
{
    public class ScoreSheetStateConverter:IValueConverter
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
                    image.UriSource = new Uri("Images/complete.png", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return image;
                case "1":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/invalid.png", UriKind.RelativeOrAbsolute);
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
