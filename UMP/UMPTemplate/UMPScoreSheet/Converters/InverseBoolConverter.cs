//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    862e461f-6355-42dc-8948-f88b5406750a
//        CLR Version:              4.0.30319.18444
//        Name:                     InverseBoolConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Converters
//        File Name:                InverseBoolConverter
//
//        created by Charley at 2014/8/7 16:21:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.UMP.ScoreSheets.Converters
{
    public class InverseBoolConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            bool bValue;
            if (bool.TryParse(value.ToString(), out bValue))
            {
                if (bValue)
                {
                    return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
