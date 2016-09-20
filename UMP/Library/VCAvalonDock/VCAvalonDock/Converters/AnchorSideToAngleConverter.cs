//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    58aa79ec-4073-42db-80cb-6e56dffbb0e4
//        CLR Version:              4.0.30319.18444
//        Name:                     AnchorSideToAngleConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                AnchorSideToAngleConverter
//
//        created by Charley at 2014/7/22 9:29:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    [ValueConversion(typeof(AnchorSide), typeof(double))]
    public class AnchorSideToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AnchorSide side = (AnchorSide)value;
            if (side == AnchorSide.Left ||
                side == AnchorSide.Right)
                return 90.0;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
