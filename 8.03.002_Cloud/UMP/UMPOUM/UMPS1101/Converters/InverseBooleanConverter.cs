//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d59321ef-5198-4845-bc38-cc21a0958191
//        CLR Version:              4.0.30319.18444
//        Name:                     InverseBooleanConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                InverseBooleanConverter
//
//        created by Charley at 2014/9/17 11:04:25
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;

namespace UMPS1101.Converters
{
    public class InverseBooleanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            if ((bool) value)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
