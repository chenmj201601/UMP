//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bab39875-8d1e-4abe-aecd-974318f90f3a
//        CLR Version:              4.0.30319.18063
//        Name:                     IsCheckedToForeground
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Converters
//        File Name:                IsCheckedToForeground
//
//        created by Charley at 2015/12/24 10:50:56
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace UMPBuilder.Converters
{
    public class IsCheckedToForeground:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool) value;
            if (isChecked)
            {
                return new SolidColorBrush(Colors.Black);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
