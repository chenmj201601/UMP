//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    612a6703-485a-47c6-949f-aa3b6b47cb2f
//        CLR Version:              4.0.30319.18408
//        Name:                     InverseBoolToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Core.Converters
//        File Name:                InverseBoolToVisibilityConverter
//
//        created by Charley at 2016/7/14 09:57:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;


namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            bool boolValue;
            if (!bool.TryParse(value.ToString(), out boolValue))
            {
                return Visibility.Visible;
            }
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
