//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    480f49e5-3124-42c2-bbd9-4f6b017ea237
//        CLR Version:              4.0.30319.18444
//        Name:                     AnchorSideToOrientationConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Converters
//        File Name:                AnchorSideToOrientationConverter
//
//        created by Charley at 2014/7/22 9:29:49
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock.Converters
{
    [ValueConversion(typeof(AnchorSide), typeof(Orientation))]
    public class AnchorSideToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AnchorSide side = (AnchorSide)value;
            if (side == AnchorSide.Left ||
                side == AnchorSide.Right)
                return Orientation.Vertical;

            return Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
