//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    db78a05b-29bb-4d29-9993-13732192fd59
//        CLR Version:              4.0.30319.18444
//        Name:                     InverseBoolConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Converters
//        File Name:                InverseBoolConverter
//
//        created by Charley at 2015/2/5 16:23:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;

namespace UMPS1110.Converters
{
    public class InverseBoolConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value.ToString();
            bool boolValue;
            if (!bool.TryParse(strValue, out boolValue))
            {
                return true;
            }
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
