//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b92d3d5e-2081-42e5-97ad-11df5f4594ea
//        CLR Version:              4.0.30319.18063
//        Name:                     LangItemStateConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Converters
//        File Name:                LangItemStateConverter
//
//        created by Charley at 2015/6/8 14:31:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UMPLanguageManager.Models;

namespace UMPLanguageManager.Converters
{
    public class LangItemStateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LangItemState state = (LangItemState)value;
            if ((state & LangItemState.ValueChanged) > 0)
            {
                return Brushes.LightCoral;
            }
            if ((state & LangItemState.Current) > 0)
            {
                return Brushes.DeepSkyBlue;
            }
            if ((state & LangItemState.Searched) > 0)
            {
                return Brushes.LightBlue;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
