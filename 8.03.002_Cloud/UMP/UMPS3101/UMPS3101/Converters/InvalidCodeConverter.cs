//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e515b3c-f958-43ac-88a0-a2aa8e80cca1
//        CLR Version:              4.0.30319.18444
//        Name:                     InvalidCodeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                InvalidCodeConverter
//
//        created by Charley at 2014/10/14 17:32:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UMPS3101.Converters
{
    public class InvalidCodeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            int intValue;
            if (int.TryParse(value.ToString(), out intValue))
            {
                if (intValue != 0)
                {
                    return string.Format("Images/invalid.png");
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
