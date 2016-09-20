//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    160bc3c5-f978-4376-8123-70be986928de
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectTypeToNameConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                ObjectTypeToNameConverter
//
//        created by Charley at 2014/7/21 11:28:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    public class ObjectTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string valueString = value.ToString();
                if (string.IsNullOrEmpty(valueString)
                 || (valueString == value.GetType().UnderlyingSystemType.ToString()))
                {
                    return value.GetType().Name;
                }
                return value;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
