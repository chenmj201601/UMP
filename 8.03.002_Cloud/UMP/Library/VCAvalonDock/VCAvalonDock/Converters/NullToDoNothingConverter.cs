//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be9e5d0a-71ea-4050-88e3-f3e0dc664b40
//        CLR Version:              4.0.30319.18444
//        Name:                     NullToDoNothingConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                NullToDoNothingConverter
//
//        created by Charley at 2014/7/22 9:32:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    public class NullToDoNothingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
