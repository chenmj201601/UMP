//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e9d4f37a-4061-4eac-8882-0fd800e98641
//        CLR Version:              4.0.30319.18444
//        Name:                     CellDatetimeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                CellDatetimeConverter
//
//        created by Charley at 2014/11/13 17:46:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class CellDatetimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return string.Empty;
            }
            try
            {
                DateTime dt = DateTime.Parse(value.ToString());
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
