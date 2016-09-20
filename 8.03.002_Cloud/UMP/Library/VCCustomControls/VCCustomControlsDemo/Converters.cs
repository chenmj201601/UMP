//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0734b691-b41e-4dcf-b91c-ff182ea1f6e3
//        CLR Version:              4.0.30319.18444
//        Name:                     Converters
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                Converters
//
//        created by Charley at 2014/8/22 11:42:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VCCustomControlsDemo
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
            catch {}
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
