//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    49d24859-9694-44ca-b2d2-df4bae420506
//        CLR Version:              4.0.30319.18408
//        Name:                     BooleanNullToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Converters
//        File Name:                BooleanNullToVisibilityConverter
//
//        created by Charley at 2016/5/4 16:43:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;


namespace UMPS1206.Converters
{
    public class BooleanNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            bool boolValue;
            if (bool.TryParse(value.ToString(), out boolValue))
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
