//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d99446be-553a-4438-a2f3-6fd8029ea11d
//        CLR Version:              4.0.30319.18444
//        Name:                     ViewClassicToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                ViewClassicToVisibilityConverter
//
//        created by Charley at 2014/8/5 16:37:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
{
    public class ViewClassicToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            string type = parameter.ToString();
            if (string.IsNullOrEmpty(type))
            {
                return Visibility.Collapsed;
            }
            ScoreItemClassic viewClassic;
            if (Enum.TryParse(value.ToString(), out viewClassic))
            {
                if (viewClassic == ScoreItemClassic.Table && type == "Table")
                {
                    return Visibility.Visible;
                }
                if (viewClassic == ScoreItemClassic.Tree && type == "Tree")
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
