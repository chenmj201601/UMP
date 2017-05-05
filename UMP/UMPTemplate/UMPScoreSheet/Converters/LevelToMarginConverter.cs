//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    64488a4c-a4d4-453f-a858-2b1fd478b1ca
//        CLR Version:              4.0.30319.18444
//        Name:                     LevelToMarginConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                LevelToMarginConverter
//
//        created by Charley at 2015/1/9 17:37:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
{
    public class LevelToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int margin = int.Parse(value.ToString()) * 15;
            return new Thickness(margin, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
