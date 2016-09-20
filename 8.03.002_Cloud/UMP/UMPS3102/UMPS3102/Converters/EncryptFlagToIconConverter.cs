//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7367b88a-40a9-47fa-86f2-56e2bf2370f6
//        CLR Version:              4.0.30319.18063
//        Name:                     EncryptFlagToIconConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                EncryptFlagToIconConverter
//
//        created by Charley at 2015/8/14 15:57:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class EncryptFlagToIconConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            string strFlag = value.ToString();
            if (strFlag == "2")
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
