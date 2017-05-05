//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c43bd4bd-2390-456b-9f68-662d01dbf5f6
//        CLR Version:              4.0.30319.18444
//        Name:                     InvalidCodeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Converters
//        File Name:                InvalidCodeConverter
//
//        created by Charley at 2014/7/24 14:37:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UMPScoreSheetDesigner.Converters
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
