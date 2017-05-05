//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    014b45e8-67ea-4fb5-bfc4-157592f597af
//        CLR Version:              4.0.30319.18444
//        Name:                     StringColorToBackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                StringColorToBackgroundConverter
//
//        created by Charley at 2014/12/12 13:58:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media;
using VoiceCyber.UMP.Common;

namespace UMPS3102.Converters
{
    public class StringColorToBackgroundConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return new SolidColorBrush(Utils.GetColorFromRgbString(value.ToString()));
            }catch{}
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
