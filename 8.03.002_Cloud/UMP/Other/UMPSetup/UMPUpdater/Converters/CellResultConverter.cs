//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    6c993fde-ae50-446f-971b-d2f02d9fd225
//        CLR Version:              4.0.30319.42000
//        Name:                     CellResultConverter
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Converters
//        File Name:                CellResultConverter
//
//        Created by Charley at 2016/8/31 15:52:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Data;


namespace UMPUpdater.Converters
{
    public class CellResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int result = (int) value;
            int param = int.Parse(parameter.ToString());
            return result == param ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
