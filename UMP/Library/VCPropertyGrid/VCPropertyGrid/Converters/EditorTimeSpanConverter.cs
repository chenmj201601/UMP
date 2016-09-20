//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1a1bcd03-4e4d-47d0-b13d-84f1e7f175f0
//        CLR Version:              4.0.30319.18444
//        Name:                     EditorTimeSpanConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Converters
//        File Name:                EditorTimeSpanConverter
//
//        created by Charley at 2014/7/23 11:55:19
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids.Converters
{
    /// <summary>
    /// Converts a TimeSpan value to a DateTime value.
    /// 
    /// This converter can be used in conjunction with a TimePicker in order 
    /// to create a TimeSpan edit control. 
    /// </summary>
    public sealed class EditorTimeSpanConverter : IValueConverter
    {
        public bool AllowNulls { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.AllowNulls && value == null)
                return null;

            TimeSpan timeSpan = (value != null) ? (TimeSpan)value : TimeSpan.Zero;
            return DateTime.Today + timeSpan;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.AllowNulls && value == null)
                return null;

            return (value != null)
              ? ((DateTime)value).TimeOfDay
              : TimeSpan.Zero;
        }
    }
}
