//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    17ca1f90-5f08-4faa-94d7-8ac3b0604712
//        CLR Version:              4.0.30319.18444
//        Name:                     MilliSecondToTimeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                MilliSecondToTimeConverter
//
//        created by Charley at 2014/12/11 11:13:11
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class MilliSecondToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double millisecond = double.Parse(value.ToString());
                int hour, minute;
                double second;
                hour = (int)(millisecond / (60 * 60 * 1000));
                millisecond = millisecond - hour*(60*60*1000);
                minute = (int) (millisecond/(60*1000));
                millisecond = millisecond - minute*(60*1000);
                second = millisecond/(1000);
                return string.Format("{0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00.0"));
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
