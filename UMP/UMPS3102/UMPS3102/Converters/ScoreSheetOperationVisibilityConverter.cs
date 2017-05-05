//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    84079241-e247-4cd4-8058-17699c7cea62
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetOperationVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                ScoreSheetOperationVisibilityConverter
//
//        created by Charley at 2014/11/16 16:17:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class ScoreSheetOperationVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int flag;
            if (int.TryParse(value.ToString(), out flag))
            {
                var param = parameter.ToString();
                if (string.IsNullOrEmpty(param))
                {
                    return Visibility.Collapsed;
                }
                if (param == "Add")
                {
                    if (flag == 0)
                    {
                        return Visibility.Visible;
                    }
                }
                if (param == "Modify")
                {
                    if (flag == 1)
                    {
                        return Visibility.Visible;
                    }
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
