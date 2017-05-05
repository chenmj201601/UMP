//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ef4d83d9-c58d-4bc6-af2a-d6c55e2e9985
//        CLR Version:              4.0.30319.18063
//        Name:                     StatusToBackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Converters
//        File Name:                StatusToBackgroundConverter
//
//        created by Charley at 2015/12/22 18:08:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UMPBuilder.Converters
{
    public class StatusToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int status = (int)value;
            if (status == UMPBuilderConsts.STA_FAIL)
            {
                return new SolidColorBrush(Colors.Red);
            }
            if (status == UMPBuilderConsts.STA_WORKING)
            {
                return new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
