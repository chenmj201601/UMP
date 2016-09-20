//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ec1db635-3464-4284-ab80-3cbf77bdae29
//        CLR Version:              4.0.30319.18063
//        Name:                     InverseBooleanToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Converters
//        File Name:                InverseBooleanToVisibilityConverter
//
//        created by Charley at 2015/4/23 19:19:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace UMPS1110.Converters
{
    public class InverseBooleanToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue = (bool) value;
            return bValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
