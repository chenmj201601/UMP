//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ac1da9ac-533e-4f58-a419-99eeaca2bea4
//        CLR Version:              4.0.30319.18063
//        Name:                     MediaTypeToImageConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                MediaTypeToImageConverter
//
//        created by Charley at 2015/7/21 15:48:35
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class MediaTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Collapsed;
            }
            int mediaType;
            string strValue = value.ToString();
            string strParam = parameter.ToString();
            if (!int.TryParse(strValue, out mediaType))
            {
                return Visibility.Collapsed;
            }
            if (mediaType == 0
                && strParam == "A")
            {
                return Visibility.Visible;
            }
            if (mediaType == 1
                && strParam=="V")
            {
                return Visibility.Visible;
            }
            if (mediaType == 2 
                && strParam == "S")
            {
                return Visibility.Visible;
            }
            if (mediaType == 3
               && strParam == "S")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
