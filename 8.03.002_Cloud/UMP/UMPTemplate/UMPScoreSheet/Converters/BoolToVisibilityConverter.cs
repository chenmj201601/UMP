//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5f422588-774c-4557-80a0-224ea86d18b0
//        CLR Version:              4.0.30319.18444
//        Name:                     BoolToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                BoolToVisibilityConverter
//
//        created by Charley at 2014/8/6 17:35:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
{
    public class BoolToVisibilityConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            bool bValue;
            if (bool.TryParse(value.ToString(), out bValue))
            {
                if (bValue)
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
