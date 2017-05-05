//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cf46192a-3ace-4486-b3dd-3fca4d803d3b
//        CLR Version:              4.0.30319.18444
//        Name:                     ViewClassicToAlignmentConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                ViewClassicToAlignmentConverter
//
//        created by Charley at 2014/8/7 11:10:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
{
    public class ViewClassicToAlignmentConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return HorizontalAlignment.Left;
            }
            ScoreItemClassic viewClassic;
            if (Enum.TryParse(value.ToString(), out viewClassic))
            {
                if (viewClassic == ScoreItemClassic.Table)
                {
                    return HorizontalAlignment.Center;
                }
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
