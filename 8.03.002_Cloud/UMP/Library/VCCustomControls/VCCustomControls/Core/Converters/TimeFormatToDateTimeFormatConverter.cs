//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    704875e4-c7ec-41bb-956f-fa0fc51d3f06
//        CLR Version:              4.0.30319.18444
//        Name:                     TimeFormatToDateTimeFormatConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                TimeFormatToDateTimeFormatConverter
//
//        created by Charley at 2014/7/17 17:57:21
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    public class TimeFormatToDateTimeFormatConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeFormat timeFormat = (TimeFormat)value;
            switch (timeFormat)
            {
                case TimeFormat.Custom:
                    return DateTimeFormat.Custom;
                case TimeFormat.ShortTime:
                    return DateTimeFormat.ShortTime;
                case TimeFormat.LongTime:
                    return DateTimeFormat.LongTime;
                default:
                    return DateTimeFormat.ShortTime;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
