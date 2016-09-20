//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    802e813f-ca2f-452a-bfa9-8c92408adb8a
//        CLR Version:              4.0.30319.18444
//        Name:                     ExpandableObjectMarginConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Converters
//        File Name:                ExpandableObjectMarginConverter
//
//        created by Charley at 2014/7/23 11:55:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids.Converters
{
    public class ExpandableObjectMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int childLevel = (int)value;
            return new Thickness(childLevel * 15, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
