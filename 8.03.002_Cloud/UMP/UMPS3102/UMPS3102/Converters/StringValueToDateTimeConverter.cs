//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    00b6d957-73b5-408c-b04b-cce25a0266f0
//        CLR Version:              4.0.30319.18063
//        Name:                     StringValueToDateTimeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                StringValueToDateTimeConverter
//
//        created by Charley at 25/8/2015 13:41:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class StringValueToDateTimeConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            DateTime dt;
            if (DateTime.TryParse(value.ToString(), out dt))
            {
                return dt;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            DateTime dt = (DateTime) value;
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
