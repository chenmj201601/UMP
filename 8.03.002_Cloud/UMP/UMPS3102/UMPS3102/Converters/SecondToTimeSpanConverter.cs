//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7085bb78-a954-4271-9c70-2969a42e5df0
//        CLR Version:              4.0.30319.18444
//        Name:                     SecondToTimeSpanConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                SecondToTimeSpanConverter
//
//        created by Charley at 2014/11/13 17:17:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using VoiceCyber.Common;

namespace UMPS3102.Converters
{
    public class SecondToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strReturn = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return strReturn;
                }
                double totalSecond = double.Parse(value.ToString());
                strReturn = Converter.Second2Time(totalSecond);
            }
            catch { }
            return strReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
