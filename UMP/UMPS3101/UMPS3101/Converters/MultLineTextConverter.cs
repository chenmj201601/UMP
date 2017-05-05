//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e4337f66-5bab-4f40-b30f-04b6ea75cdd0
//        CLR Version:              4.0.30319.18444
//        Name:                     MultLineTextConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                MultLineTextConverter
//
//        created by Charley at 2014/10/14 17:32:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UMPS3101.Converters
{
    public class MultLineTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            string str = value.ToString();
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
