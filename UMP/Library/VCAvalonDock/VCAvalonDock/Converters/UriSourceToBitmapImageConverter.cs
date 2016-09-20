//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7774722e-9ff9-4c5a-bb5d-2010dc8898a8
//        CLR Version:              4.0.30319.18444
//        Name:                     UriSourceToBitmapImageConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                UriSourceToBitmapImageConverter
//
//        created by Charley at 2014/7/22 9:33:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    public class UriSourceToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;
            //return (Uri)value;
            return new Image() { Source = new BitmapImage((Uri)value) };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
