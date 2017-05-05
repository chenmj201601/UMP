//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e9ea565-af9c-4346-97ec-90a216766cb7
//        CLR Version:              4.0.30319.18444
//        Name:                     SecondToTimeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                SecondToTimeConverter
//
//        created by Charley at 2014/12/11 17:13:13
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using VoiceCyber.Common;

namespace UMPS3102.Converters
{
    public class SecondToTimeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return Converter.Second2Time(double.Parse(value.ToString()));
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
