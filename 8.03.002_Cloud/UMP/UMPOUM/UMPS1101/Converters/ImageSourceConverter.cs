//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a3233d3a-e49d-46d7-8bcd-9c139758fe5e
//        CLR Version:              4.0.30319.18444
//        Name:                     ImageSourceConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                ImageSourceConverter
//
//        created by Charley at 2014/8/26 18:10:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS1101.Converters
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
