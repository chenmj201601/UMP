//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7f717e6c-f94a-49b6-832b-5e45f0ade63d
//        CLR Version:              4.0.30319.18444
//        Name:                     MultLineTextConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Converters
//        File Name:                MultLineTextConverter
//
//        created by Charley at 2014/7/26 23:04:57
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UMPScoreSheetDesigner.Converters
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
