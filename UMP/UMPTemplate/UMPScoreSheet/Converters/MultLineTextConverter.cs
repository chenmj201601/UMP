//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    43a08868-7781-422b-a6bf-850e3ee94f3b
//        CLR Version:              4.0.30319.18444
//        Name:                     MultLineTextConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                MultLineTextConverter
//
//        created by Charley at 2014/7/28 10:46:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
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
            str = str.Replace("\r", " ");
            str = str.Replace("\n", " ");
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
